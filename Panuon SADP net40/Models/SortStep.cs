using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panuon_SADP_net40.Models
{
    /// <summary>
    /// 步骤类型
    /// </summary>
    public enum StepCategory
    {
        /// <summary>
        /// 移动单个条的位置，并**直接修改**(不是交换)对应的索引。
        /// </summary>
        MoveBar = 1,
        /// <summary>
        /// 移动指示器。使用From值（1或2）表示指示器。
        /// </summary>
        MoveIndivator = 2,
        /// <summary>
        /// 互换两个条的位置并交换索引。
        /// </summary>
        SwapBar = 3,
        /// <summary>
        /// 修改索引组，用于归并排序
        /// </summary>
        ChangeIndex = 4,
        /// <summary>
        /// 创建桶
        /// </summary>
        CreateBucket = 5,
        /// <summary>
        /// 完成
        /// </summary>
        Completed = 6,
    }
    public class SortStep
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public StepCategory Category { get; set; }

        /// <summary>
        /// 从
        /// </summary>
        public int From { get; set; }

        /// <summary>
        /// 至
        /// </summary>
        public int To { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 要更改颜色的索引。-1表示无操作，-2表示全部清空。
        /// </summary>
        public int SelectedIndex { get; set; } = -1;

        /// <summary>
        /// 是否升至临时区 1上升 0不操作 -1下降
        /// </summary>
        public int Raise { get; set; }

        /// <summary>
        /// 该步骤是否不需要挂起线程
        /// </summary>
        public bool SkipWaiting { get; set; }

        /// <summary>
        /// 修改索引集合，用于归并排序
        /// </summary>
        public int[] ChangedIndexList { get; set; }

        /// <summary>
        /// 生成桶的数量
        /// </summary>
        public int BucketQuantity { get; set; }

        /// <summary>
        /// 每个桶的大小
        /// </summary>
        public int[] BucketVolumes { get; set; }

    }

}
