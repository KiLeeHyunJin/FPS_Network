using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace YoutubePlayer
{
    public class OnlineVideoLoader : MonoBehaviour
    {
        YoutubePlayer youtubePlayer;
        VideoPlayer videoPlayer;
        public RenderTexture renderTexture;
        RawImage rawImage;
        public string cachedVideoPath = "CachedVideo.mp4";
        public string url;
        private void Awake()
        {
            videoPlayer = GetComponent<VideoPlayer>(); //비디오 플레이어를 가져온다.
            videoPlayer.url = ""; //url주소를 초기화한다.
            rawImage = GetComponent<RawImage>(); //출력할 이미지를 가져온다.
            if (rawImage.texture == null) //출력 이미지가 비어있다면
                rawImage.texture = renderTexture; //랜더 이미지를 대입한다.
        }
        private void OnEnable()
        {
            ClearRenderTexture(Color.black); // 초기화할 색상을 넣는다.
            videoPlayer.time = 0; //프레임을 0으로 초기화한다.
            if (videoPlayer.url == "")
            {
                youtubePlayer = gameObject.AddComponent<YoutubePlayer>(); //유튜브플레이어 컴포넌트를 넣는다.
                youtubePlayer.youtubeUrl = url; //스트리밍 url로 변환시킬 url을 대입한다.
                youtubePlayer.cli = YoutubePlayer.Cli.YtDlp;
            }
            Prepare();
        }

        public async void Prepare()
        {
            try
            {
                if (youtubePlayer != null)
                    await youtubePlayer.PrepareVideoAsync(); //비동기로 무언가를 실행한다.(깃허브에서 가져온거라 작동 원리를 모름.)
                StartCoroutine(Play()); //실행
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }

        public IEnumerator Play()
        {
            while (true)
            {
                yield return null;
                if (videoPlayer == null)
                    break;
                videoPlayer.Stop(); //실행중인게 있다면 정지한다.
                videoPlayer.Play(); //실행한다.
                break;
            }
        }


        void ClearRenderTexture(Color clearColor)
        {
            // RenderTexture를 현재 활성화된 RenderTexture로 설정합니다.
            RenderTexture.active = renderTexture;

            // clearColor로 RenderTexture를 초기화합니다.
            GL.Clear(true, true, clearColor);

            // RenderTexture.active를 다시 null로 설정합니다.
            RenderTexture.active = null;
        }
        private void OnDisable()
        {
            videoPlayer?.Stop(); //재생을 멈춘다.
            videoPlayer.time = 0; //프레임을 0으로 변경한다.
        }
    }
}
