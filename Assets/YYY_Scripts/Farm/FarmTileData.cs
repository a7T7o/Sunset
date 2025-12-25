using UnityEngine;

namespace FarmGame.Farm
{
    /// <summary>
    /// 单个耕地格子的数据
    /// </summary>
    [System.Serializable]
    public class FarmTileData
    {
        /// <summary>
        /// 格子在Tilemap中的坐标
        /// </summary>
        public Vector3Int position;

        /// <summary>
        /// 是否已耕作
        /// </summary>
        public bool isTilled;

        /// <summary>
        /// 今天是否已浇水（记录参数，第二天才生效）
        /// </summary>
        public bool wateredToday;

        /// <summary>
        /// 昨天是否浇过水（实际影响作物生长）
        /// </summary>
        public bool wateredYesterday;

        /// <summary>
        /// 浇水的游戏时间（小时，用于计算视觉状态）
        /// </summary>
        public float waterTime;

        /// <summary>
        /// 当前土壤湿度视觉状态
        /// </summary>
        public SoilMoistureState moistureState;

        /// <summary>
        /// 当前种植的作物实例
        /// </summary>
        public CropInstance crop;

        public FarmTileData(Vector3Int pos)
        {
            position = pos;
            isTilled = false;
            wateredToday = false;
            wateredYesterday = false;
            waterTime = -1f;
            moistureState = SoilMoistureState.Dry;
            crop = null;
        }

        /// <summary>
        /// 是否可以种植
        /// </summary>
        public bool CanPlant()
        {
            return isTilled && crop == null;
        }

        /// <summary>
        /// 是否有作物
        /// </summary>
        public bool HasCrop()
        {
            return crop != null && crop.cropObject != null;
        }

        /// <summary>
        /// 清除作物数据
        /// </summary>
        public void ClearCrop()
        {
            if (crop != null && crop.cropObject != null)
            {
                Object.Destroy(crop.cropObject);
            }
            crop = null;
        }
    }
}
