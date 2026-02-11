using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

namespace FarmGame.Farm
{
    /// <summary>
    /// 农田视觉管理器
    /// 负责 Tile 视觉更新、音效和粒子效果
    /// </summary>
    public class FarmVisualManager : MonoBehaviour
    {
        #region 单例
        
        public static FarmVisualManager Instance { get; private set; }
        
        #endregion

        #region Tile 资源配置
        
        [Header("耕地 Tile")]
        [Tooltip("干燥状态的耕地 Tile（Rule Tile）")]
        [SerializeField] private TileBase dryFarmlandTile;
        
        [Tooltip("湿润深色状态的耕地 Tile")]
        [SerializeField] private TileBase wetDarkTile;
        
        [Header("水渍 Tile")]
        [Tooltip("水渍 Tile 变体（3 种）")]
        [SerializeField] private TileBase[] wetPuddleTiles;
        
        #endregion

        #region 音效配置
        
        [Header("音效")]
        [SerializeField] private AudioClip tillingSoundClip;
        [SerializeField] private AudioClip wateringSoundClip;
        [SerializeField] private AudioClip harvestSoundClip;
        [SerializeField] private AudioClip plantingSoundClip;
        
        #endregion

        #region 粒子效果配置
        
        [Header("粒子效果")]
        [SerializeField] private GameObject tillingParticlePrefab;
        [SerializeField] private GameObject wateringParticlePrefab;
        [SerializeField] private GameObject harvestParticlePrefab;
        
        [Header("粒子效果设置")]
        [Tooltip("粒子效果持续时间")]
        [SerializeField] private float particleDuration = 2f;
        
        [Tooltip("对象池初始大小")]
        [SerializeField] private int poolInitialSize = 5;
        
        #endregion

        #region Debug
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = false;
        
        #endregion

        #region 内部变量
        
        private AudioSource audioSource;
        
        // 粒子效果对象池
        private Queue<GameObject> tillingParticlePool;
        private Queue<GameObject> wateringParticlePool;
        private Queue<GameObject> harvestParticlePool;
        
        #endregion

        #region 生命周期
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeAudio();
                InitializeParticlePools();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeAudio()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        private void InitializeParticlePools()
        {
            tillingParticlePool = new Queue<GameObject>();
            wateringParticlePool = new Queue<GameObject>();
            harvestParticlePool = new Queue<GameObject>();
            
            // 预创建对象池
            PrewarmPool(tillingParticlePrefab, tillingParticlePool, poolInitialSize);
            PrewarmPool(wateringParticlePrefab, wateringParticlePool, poolInitialSize);
            PrewarmPool(harvestParticlePrefab, harvestParticlePool, poolInitialSize);
        }
        
        private void PrewarmPool(GameObject prefab, Queue<GameObject> pool, int count)
        {
            if (prefab == null) return;
            
            for (int i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(prefab, transform);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
        }
        
        #endregion

        #region Tile 视觉更新
        
        /// <summary>
        /// 更新耕地 Tile 视觉
        /// </summary>
        /// <param name="tilemaps">楼层 Tilemap 配置</param>
        /// <param name="cellPosition">格子坐标</param>
        /// <param name="tileData">耕地数据</param>
        public void UpdateTileVisual(LayerTilemaps tilemaps, Vector3Int cellPosition, FarmTileData tileData)
        {
            if (tilemaps == null) return;
            
            // 🔥 Bug C 修复：优先使用新版字段
            Tilemap farmTilemap = tilemaps.farmlandCenterTilemap;
            #pragma warning disable 0618
            if (farmTilemap == null) farmTilemap = tilemaps.farmlandTilemap;
            #pragma warning restore 0618
            
            if (farmTilemap == null) return;
            
            // 🔥 Bug C 修复：优先使用新版水渍 Tilemap
            Tilemap puddleTilemap = tilemaps.waterPuddleTilemapNew;
            #pragma warning disable 0618
            if (puddleTilemap == null) puddleTilemap = tilemaps.waterPuddleTilemap;
            #pragma warning restore 0618
            
            TileBase puddleTile = null;
            
            switch (tileData.moistureState)
            {
                case SoilMoistureState.Dry:
                    puddleTile = null; // 清除水渍
                    break;
                    
                case SoilMoistureState.WetWithPuddle:
                    // 水渍在叠加层显示
                    if (wetPuddleTiles != null && wetPuddleTiles.Length > 0)
                    {
                        int variant = Mathf.Clamp(tileData.puddleVariant, 0, wetPuddleTiles.Length - 1);
                        puddleTile = wetPuddleTiles[variant];
                    }
                    break;
                    
                case SoilMoistureState.WetDark:
                    puddleTile = null; // 清除水渍
                    break;
            }
            
            // 更新水渍叠加层
            if (puddleTilemap != null)
            {
                puddleTilemap.SetTile(cellPosition, puddleTile);
            }
            
            if (showDebugInfo)
                Debug.Log($"[FarmVisualManager] 更新 Tile 视觉: Pos={cellPosition}, State={tileData.moistureState}, puddleTilemap={(puddleTilemap != null ? "有" : "null")}");
        }
        
        /// <summary>
        /// 清除耕地 Tile（用于移除耕地时）
        /// </summary>
        public void ClearTileVisual(LayerTilemaps tilemaps, Vector3Int cellPosition)
        {
            if (tilemaps == null) return;
            
            // 🔥 Bug C 修复：优先使用新版字段，回退到旧版
            Tilemap farmTilemap = tilemaps.farmlandCenterTilemap;
            #pragma warning disable 0618
            if (farmTilemap == null) farmTilemap = tilemaps.farmlandTilemap;
            #pragma warning restore 0618
            if (farmTilemap != null)
            {
                farmTilemap.SetTile(cellPosition, null);
            }
            
            // 🔥 Bug C 修复：优先使用新版水渍 Tilemap
            Tilemap puddleTilemap = tilemaps.waterPuddleTilemapNew;
            #pragma warning disable 0618
            if (puddleTilemap == null) puddleTilemap = tilemaps.waterPuddleTilemap;
            #pragma warning restore 0618
            if (puddleTilemap != null)
            {
                puddleTilemap.SetTile(cellPosition, null);
            }
        }
        
        #endregion

        #region 音效播放
        
        /// <summary>
        /// 播放锄地音效
        /// </summary>
        public void PlayTillingSound()
        {
            PlaySound(tillingSoundClip);
        }
        
        /// <summary>
        /// 播放浇水音效
        /// </summary>
        public void PlayWateringSound()
        {
            PlaySound(wateringSoundClip);
        }
        
        /// <summary>
        /// 播放收获音效
        /// </summary>
        public void PlayHarvestSound()
        {
            PlaySound(harvestSoundClip);
        }
        
        /// <summary>
        /// 播放种植音效
        /// </summary>
        public void PlayPlantingSound()
        {
            PlaySound(plantingSoundClip);
        }
        
        private void PlaySound(AudioClip clip)
        {
            if (clip != null && audioSource != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
        
        #endregion

        #region 粒子效果
        
        /// <summary>
        /// 播放锄地效果（音效 + 粒子）
        /// </summary>
        public void PlayTillingEffects(Vector3 worldPosition)
        {
            PlayTillingSound();
            SpawnParticle(tillingParticlePrefab, tillingParticlePool, worldPosition);
        }
        
        /// <summary>
        /// 播放浇水效果（音效 + 粒子）
        /// </summary>
        public void PlayWateringEffects(Vector3 worldPosition)
        {
            PlayWateringSound();
            SpawnParticle(wateringParticlePrefab, wateringParticlePool, worldPosition);
        }
        
        /// <summary>
        /// 播放收获效果（音效 + 粒子）
        /// </summary>
        public void PlayHarvestEffects(Vector3 worldPosition)
        {
            PlayHarvestSound();
            SpawnParticle(harvestParticlePrefab, harvestParticlePool, worldPosition);
        }
        
        private void SpawnParticle(GameObject prefab, Queue<GameObject> pool, Vector3 position)
        {
            if (prefab == null) return;
            
            GameObject particle = GetFromPool(prefab, pool);
            particle.transform.position = position;
            particle.SetActive(true);
            
            // 延迟回收
            StartCoroutine(ReturnToPoolDelayed(particle, pool, particleDuration));
        }
        
        #endregion

        #region 对象池
        
        private GameObject GetFromPool(GameObject prefab, Queue<GameObject> pool)
        {
            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
            
            // 池中没有可用对象，创建新的
            return Instantiate(prefab, transform);
        }
        
        private IEnumerator ReturnToPoolDelayed(GameObject obj, Queue<GameObject> pool, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (obj != null)
            {
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
        }
        
        #endregion

        #region 批量更新
        
        /// <summary>
        /// 批量更新所有耕地的视觉状态
        /// </summary>
        public void RefreshAllTileVisuals()
        {
            if (FarmTileManager.Instance == null) return;
            
            int updatedCount = 0;
            
            for (int i = 0; i < FarmTileManager.Instance.LayerCount; i++)
            {
                var tilemaps = FarmTileManager.Instance.GetLayerTilemaps(i);
                if (tilemaps == null) continue;
                
                foreach (var tileData in FarmTileManager.Instance.GetAllTilesInLayer(i))
                {
                    if (tileData.isTilled)
                    {
                        UpdateTileVisual(tilemaps, tileData.position, tileData);
                        updatedCount++;
                    }
                }
            }
            
            if (showDebugInfo)
                Debug.Log($"[FarmVisualManager] 刷新所有 Tile 视觉: {updatedCount} 块");
        }
        
        #endregion
    }
}
