using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace State.Machine
{
    public class SituationManager : StateMachineUpdate
    {
        [SerializeField] private CinemachineBrain _cameraBrain;
        [SerializeField] private float _openingTimer = 2f;
        [SerializeField] private float _endingTimer = 3f;

        [Header("シチュエーション開始時にONになる")]
        [SerializeField] private List<GameObject> _waitingEnableObjects;

        [SerializeField] private List<GameObject> _openingEnableObjects;
        [SerializeField] private List<GameObject> _playingEnableObjects;
        [SerializeField] private List<GameObject> _endingEnableObjects;
        [SerializeField] private List<GameObject> _exitEnableObjects;

        [Header("シチュエーション終了時にOFFになる")]
        [SerializeField] private List<GameObject> _waitingDisableObjects;

        [SerializeField] private List<GameObject> _openingDisableObjects;
        [SerializeField] private List<GameObject> _playingDisableObjects;
        [SerializeField] private List<GameObject> _endingDisableObjects;
        [SerializeField] private List<GameObject> _exitDisableObjects;

        [Header("シチュエーションのカメラ移動後ONになる")]
        [SerializeField] private List<GameObject> _waitingBlendFixEnableObjects;

        [SerializeField] private List<GameObject> _openingBlendFixEnableObjects;
        [SerializeField] private List<GameObject> _playingBlendFixEnableObjects;
        [SerializeField] private List<GameObject> _endingBlendFixEnableObjects;
        [SerializeField] private List<GameObject> _exitBlendFixEnableObjects;

        [Header("シチュエーションのカメラ移動後OFFになる")]
        [SerializeField] private List<GameObject> _waitingBlendFixDisableObjects;

        [SerializeField] private List<GameObject> _openingBlendFixDisableObjects;
        [SerializeField] private List<GameObject> _playingBlendFixDisableObjects;
        [SerializeField] private List<GameObject> _endingBlendFixDisableObjects;
        [SerializeField] private List<GameObject> _exitBlendFixDisableObjects;

        public bool IsGameExit { get; private set; } = false;
        private bool _isFixCameraBlend;

        public Action OnCameraChange { get; set; }

        public enum Mode
        {
            Waiting,
            Opening,
            Playing,
            Ending,
            Exit
        }


        private void Awake()
        {
            if (!_cameraBrain)
            {
                _cameraBrain = FindObjectOfType<CinemachineBrain>();
            }

            //SetActive(_waitingEnableObjects, false);最初のモードは必ずWaitingなので、消す必要がない
            SetActives(_openingEnableObjects, false);
            SetActives(_playingEnableObjects, false);
            SetActives(_endingEnableObjects, false);
            SetActives(_exitEnableObjects, false);
            SetActives(_waitingBlendFixEnableObjects, false);
            SetActives(_openingBlendFixEnableObjects, false);
            SetActives(_playingBlendFixEnableObjects, false);
            SetActives(_endingBlendFixEnableObjects, false);
            SetActives(_exitBlendFixEnableObjects, false);
            InitStateObjects();
            ChangeMode((int) Mode.Waiting);
        }


        /// <summary>
        /// それぞれの状態の時に、対応したオブジェクトをONにする
        /// </summary>
        private void InitStateObjects()
        {
            //待機中
            AddEvents((int) Mode.Waiting,
                () =>
                {
                    _isFixCameraBlend = false;
                    SetActives(_waitingEnableObjects, true);
                },
                () => CallEventIfCameraBlendCompleted(_waitingBlendFixEnableObjects, _waitingBlendFixDisableObjects),
                () => SetActives(_waitingDisableObjects, false));
            //オープニング
            AddEvents((int) Mode.Opening,
                () =>
                {
                    _isFixCameraBlend = false;
                    SetActives(_openingEnableObjects, true);

                },
                () =>
                {
                    CallEventIfCameraBlendCompleted(_openingBlendFixEnableObjects, _openingBlendFixDisableObjects);

                    _openingTimer -= Time.deltaTime;
                    if (_openingTimer > 0)
                    {
                        return;
                    }

                    ChangeMode((int) Mode.Playing);

                },
                () => SetActives(_openingDisableObjects, false));

            //プレイ中
            AddEvents((int) Mode.Playing,
                () =>
                {
                    _isFixCameraBlend = false;
                    SetActives(_playingEnableObjects, true);

                },
                () => CallEventIfCameraBlendCompleted(_playingBlendFixEnableObjects, _playingBlendFixDisableObjects),
                () => SetActives(_playingDisableObjects, false));

            //クリア
            AddEvents((int) Mode.Ending,
                () =>
                {
                    _isFixCameraBlend = false;
                    SetActives(_endingEnableObjects, true);
                },
                () =>
                {
                    CallEventIfCameraBlendCompleted(_endingBlendFixEnableObjects, _endingBlendFixDisableObjects);

                    _endingTimer -= Time.deltaTime;
                    if (_endingTimer > 0)
                    {
                        return;
                    }

                    ChangeMode((int) Mode.Exit);
                },
                () => SetActives(_endingDisableObjects, false));

            //ゲーム演出を終了
            AddEvents((int) Mode.Exit,
                () =>
                {
                    _isFixCameraBlend = false;
                    IsGameExit = true;
                    SetActives(_exitEnableObjects, true);
                    return;
                },
                () => CallEventIfCameraBlendCompleted(_exitBlendFixEnableObjects, _exitBlendFixDisableObjects),
                () => SetActives(_exitDisableObjects, false));
        }


        /// <summary>
        /// Virtual Camera Brainのカメラブレンドが終了していれば、該当のオブジェクト群のOn,Offを行う
        /// </summary>
        /// <param name="enableObjects"></param>
        /// <param name="disableObjects"></param>
        private UniTaskVoid CallEventIfCameraBlendCompleted(IEnumerable<GameObject> enableObjects,
            IEnumerable<GameObject> disableObjects)
        {
            if (_cameraBrain.IsBlending || _isFixCameraBlend)
            {
                return default;
            }

            OnCameraChange?.Invoke();
            OnCameraChange = null;

            _isFixCameraBlend = true;
            SetActives(enableObjects, true);
            SetActives(disableObjects, false);
            return default;
        }


        /// <summary>
        /// 渡したゲームオブジェクトのアクティブ状態を一括で変更する
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="flag"></param>
        private static UniTaskVoid SetActives(IEnumerable<GameObject> objects, bool flag)
        {
            foreach (var o in objects)
            {
                o.SetActive(flag);
            }
            return default;
        }


        /// <summary>
        /// ゲーム開始、外部から呼ぶ
        /// </summary>
        public void GameStart()
        {
            //ToDo:関数名をExternalGameStartに変更したいけれど、他のソースに影響がある為、タイミングを測って行う
            ChangeMode((int) Mode.Opening);
        }
    }
}
