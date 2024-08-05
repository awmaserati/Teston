using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Teston.Utils;
using UnityEngine.Networking;
using System.Text;

namespace Teston.Net
{
    [Serializable]
    public class EventService : Singleton<EventService>
    {
        private const string _sendFileName = "sendevents";
        private const string _batchFileName = "batchevents";

        [SerializeField]
        private string _serverUrl = "www.google.com";
        [SerializeField]
        private float _cooldownBeforeSend;

        private EventsPool _batchEvents;
        private EventsPool _sendEvents;
        private float _currentCooldown;

        #region UnityMEFs

        protected override void Awake()
        {
            base.Awake();

            //_sendEvents = new EventsPool();
            //_sendEvents.Events.Add(new EventData("1", "3"));
            //JSONSerializer<EventsPool>.Save(_sendEvents, _sendFileName);
            //_batchEvents = new EventsPool();
            //_batchEvents.Events.Add(new EventData("3", "1"));
            //JSONSerializer<EventsPool>.Save(_batchEvents, _batchFileName);
            //_sendEvents = null;
            //_batchEvents = null;
            //JSONSerializer<EventsPool>.Load(ref _sendEvents, _sendFileName);
            //JSONSerializer<EventsPool>.Load(ref _batchEvents, _batchFileName);
            //return;
            JSONSerializer<EventsPool>.Load(ref _sendEvents, _sendFileName);

            if(_sendEvents != null)
            {
                StartCoroutine(Post(_sendEvents));
            }
            else
            {
                _sendEvents = new EventsPool();
            }

            JSONSerializer<EventsPool>.Load(ref _batchEvents, _batchFileName);

            if (_batchEvents != null && _sendEvents.Events.Count == 0)
            {
                StartCoroutine(Post(_sendEvents));
            }
            else if (_batchEvents == null)
            {
                _batchEvents = new EventsPool();
            }
        }

        #endregion

        #region MEFs

        public void TrackEvent(string type, string data)
        {
            var newEvent = new EventData(type, data);

            if(_sendEvents.Events.Count == 0)
            {
                _sendEvents.Events.Add(newEvent);
                JSONSerializer<EventsPool>.Save(_sendEvents, _sendFileName);
                StartCoroutine(Post(_sendEvents));
                return;
            }

            _batchEvents.Events.Add(newEvent);
            JSONSerializer<EventsPool>.Save(_batchEvents, _batchFileName);
        }

        private IEnumerator Post(EventsPool events)
        {
            var json = JSONSerializer<EventsPool>.GetJson(events);

            using (var request = new UnityWebRequest(_serverUrl, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();
                
                if(request.result == UnityWebRequest.Result.Success)
                {
                    JSONSerializer<EventsPool>.Delete(_sendFileName);

                    if (_currentCooldown == 0)
                    {
                        StartCoroutine(StartCooldown());
                    }
                    else if(_batchEvents.Events.Count > 0)
                    {
                        var sendEvent = _batchEvents.Events[0];

                        TrackEvent(sendEvent.Type, sendEvent.Data);
                        _batchEvents.Events.RemoveAt(0);

                        if (_batchEvents.Events.Count > 0)
                        {
                            JSONSerializer<EventsPool>.Save(_batchEvents, _batchFileName);
                        }
                        else
                        {
                            JSONSerializer<EventsPool>.Delete(_batchFileName);
                        }
                    }
                    else
                    {
                        _sendEvents.Events.Clear();
                        JSONSerializer<EventsPool>.Delete(_batchFileName);
                    }
                }
                else
                {
                    StartCoroutine(Post(_sendEvents));
                }
            }
        }

        private IEnumerator StartCooldown()
        {
            do
            {
                yield return null;
                _currentCooldown += Time.deltaTime;

            } while (_currentCooldown < _cooldownBeforeSend);

            if (_batchEvents.Events.Count > 0)
            {
                _sendEvents.Events.Clear();
                _sendEvents.Events.AddRange(_batchEvents.Events);
                _batchEvents.Events.Clear();
                JSONSerializer<EventsPool>.Save(_sendEvents, _sendFileName);
                JSONSerializer<EventsPool>.Delete(_batchFileName);
                yield return StartCoroutine(Post(_sendEvents));
            }

            _currentCooldown = 0.0f;
        }

        #endregion
    }
}
