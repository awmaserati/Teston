using Teston.Net;
using UnityEngine;
using UnityEngine.UI;

namespace Teston.UI
{
    public class UI : MonoBehaviour
    {
        [SerializeField]
        private Button _playBtn = null;
        [SerializeField]
        private Button _rewardBtn = null;
        [SerializeField]
        private Button _buyBtn = null;

        private int _levelNum = 1;

        #region UnityMEFs

        private void Awake()
        {
            _playBtn.onClick.AddListener(OnPlayClicked);
            _rewardBtn.onClick.AddListener(OnRewardClicked);
            _buyBtn.onClick.AddListener(OnBuyClicked);
        }

        #endregion

        #region MEFs

        private void OnPlayClicked()
        {
            EventService.Instance.TrackEvent("levelStart", string.Format("level:{0}", _levelNum++));
        }

        private void OnRewardClicked()
        {
            EventService.Instance.TrackEvent("getReward", "coins:200");
        }

        private void OnBuyClicked() 
        {
            EventService.Instance.TrackEvent("buyItem", "icecream");
        }

        #endregion
    }
}
