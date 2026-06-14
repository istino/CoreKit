using UnityEngine;

namespace CoreKit.Sample
{
    // 블루프린트(Authoring 데이터) — Mono 서비스의 인스펙터 설정을 담는 재사용 가능한 에셋.
    // 메뉴: Assets > Create > CoreKit Sample > Audio Config
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "CoreKit Sample/Audio Config")]
    public sealed class AudioConfig : ScriptableObject
    {
        [Range(0f, 1f)] public float masterVolume = 1f;
        public float fadeInSeconds = 2f;
    }
}
