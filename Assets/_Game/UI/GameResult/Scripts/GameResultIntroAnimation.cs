using System;
using System.Text;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

namespace _Game.UI.GameResult.Scripts
{
    [RequireComponent(typeof(PlayableDirector))]
    public class GameResultIntroAnimation : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI _infoText;
        [SerializeField] private TextMeshProUGUI _awardText;
    
        private const string VICTORY_TEXT = "LEVEL COMPLETE";
        private const string DEFEAT_TEXT = "GAME OVER";
        
        [SerializeField] private float _infoTextDelay = 1f; 
        [SerializeField] private float _infoTextDuration = 2f; 
    
        [SerializeField] private float _awardTextDelay = 1f; 
        [SerializeField] private float _awardTextDuration = 1f;
        
        [SerializeField] private PlayableDirector _director;
        private UniTaskCompletionSource<bool> _playAwaiter;

        public async UniTask Play(int award, GameResultType type)
        {
            Cleanup();
            _playAwaiter = new UniTaskCompletionSource<bool>();
            _director.stopped -= OnTimelineFinished;
            _director.stopped += OnTimelineFinished;
        
            _director.Play();
            
            await UniTask.Delay(TimeSpan.FromSeconds(_infoTextDelay));

            switch (type)
            {
                case GameResultType.Victory:
                    await AnimateInfoText(VICTORY_TEXT, _infoTextDuration);
                    break;
                case GameResultType.Defeat:
                    await AnimateInfoText(DEFEAT_TEXT, _infoTextDuration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_awardTextDelay));
            await AnimateAwardText(award, _awardTextDuration );
            await _playAwaiter.Task;
        }

        private void Cleanup()
        {
            _infoText.text = string.Empty;
            _awardText.text = string.Empty;
        }

        private async UniTask AnimateInfoText(string infoText, float infoTextDuration)
        {
            float timer = 0f;
            int totalCharacters = infoText.Length;
            StringBuilder animatedText = new StringBuilder(infoText.Length);
    
            while (timer < infoTextDuration)
            {
                float percentageComplete = timer /infoTextDuration;
                int charactersShown = (int)(totalCharacters * percentageComplete);
    
                animatedText.Length = 0; 
                animatedText.Append(infoText, 0, charactersShown + 1);

                _infoText.text = animatedText.ToString();
                
                timer += Time.deltaTime;
                await UniTask.Yield();
            }
        
            _infoText.text = infoText;
        }
    
        private async UniTask AnimateAwardText(int award, float awardTextDuration)
        {
            float timer = 0f;
    
            while (timer < awardTextDuration)
            {
                float percentageComplete = timer / awardTextDuration;
                int currentAward = Mathf.RoundToInt(Mathf.Lerp(0, award, percentageComplete));

                _awardText.text = $"+{currentAward}";

                timer += Time.deltaTime;
                await UniTask.Yield();
            }

            _awardText.text = $"+{award}";
        }
        
        private void OnTimelineFinished(PlayableDirector _)
        {
            _playAwaiter.TrySetResult(true);
        }
    }
}
