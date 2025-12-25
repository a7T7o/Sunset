using UnityEngine;

namespace FarmGame.Farm
{
    /// <summary>
    /// 作物控制器 - 附加到单个作物GameObject上
    /// 负责作物的渲染和交互
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class CropController : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private CropInstance cropInstance;

        // 成熟时的闪烁效果
        [Header("=== 成熟特效 ===")]
        [SerializeField] private bool enableMatureGlow = true;
        [SerializeField] private float glowSpeed = 2f;
        [SerializeField] private Color glowColor = new Color(1f, 1f, 0.8f, 1f);

        private bool isMature = false;
        private float glowTime = 0f;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// 初始化作物
        /// </summary>
        public void Initialize(CropInstance instance)
        {
            cropInstance = instance;
            UpdateVisuals();
        }

        /// <summary>
        /// 更新作物外观
        /// </summary>
        public void UpdateVisuals()
        {
            if (cropInstance == null || spriteRenderer == null)
                return;

            // 更新Sprite
            Sprite sprite = cropInstance.GetCurrentSprite();
            if (sprite != null)
            {
                spriteRenderer.sprite = sprite;
            }

            // 更新颜色
            if (cropInstance.isWithered)
            {
                // 枯萎：黄褐色
                spriteRenderer.color = new Color(0.8f, 0.7f, 0.4f, 1f);
                isMature = false;
            }
            else
            {
                // 正常颜色
                spriteRenderer.color = Color.white;
                
                // 检查是否成熟
                isMature = cropInstance.IsMature();
            }
        }

        private void Update()
        {
            // 成熟作物的闪烁效果
            if (isMature && enableMatureGlow && !cropInstance.isWithered)
            {
                glowTime += Time.deltaTime * glowSpeed;
                float glow = Mathf.PingPong(glowTime, 1f);
                spriteRenderer.color = Color.Lerp(Color.white, glowColor, glow * 0.3f);
            }
        }

        private void OnMouseOver()
        {
            // TODO: 显示作物信息UI
            // 例如：作物名称、生长进度、是否可收获等
        }

        private void OnMouseExit()
        {
            // TODO: 隐藏作物信息UI
        }
    }
}
