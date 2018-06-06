using Panuon_SADP_net40.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panuon_SADP_net40.Logic
{
    public class SortStepGenerator
    {
        public static IEnumerable<SortStep> Sort(SortCategory sortMode, IEnumerable<int> data)
        {
            switch (sortMode)
            {
                case SortCategory.InsertSort:
                    return InsertSort(data);
                case SortCategory.SelectSort:
                    return SelectSort(data);
                case SortCategory.BubbleSort:
                    return BubbleSort(data);
                case SortCategory.ShellSort:
                    return ShellSort(data);
                case SortCategory.QucikSort:
                    return QuickSort(data);
                case SortCategory.MergeSort:
                    return MergeSort(data);
                case SortCategory.BucketSort:
                    return BucketSort(data);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 插入排序。
        /// </summary>
        /// <param name="data">要排序的集合。</param>
        private static IEnumerable<SortStep> InsertSort(IEnumerable<int> data)
        {
            count = 1;
            var array = data.ToArray();
            for (var i = 1; i < array.Length; i++)
            {
                yield return GetSortStep(StepCategory.MoveIndivator, 1, i, $"[{count++}]将指示器A移动至{i + 1}格。");
                for (var j = i; j > 0; j--)
                {
                    yield return GetSortStep(StepCategory.MoveIndivator, 2, j - 1, $"[{count++}]将指示器B移动至{j}格，寻找比{ array[j] }更大的值", j);
                    if (array[j] < array[j - 1])
                    {
                        var temp = array[j];
                        array[j] = array[j - 1];
                        array[j - 1] = temp;
                        yield return GetSortStep(StepCategory.SwapBar, j, j - 1, $"[{count++}]找到一个较大值{ array[j - 1] }，交换它与{ array[j]}的位置。", j - 1);
                    }
                    else
                        break;
                }
            }
            yield return GetSortStep(StepCategory.Completed, 0, 0, $"排序已完成，共{count - 1}步。点击停止按钮将恢复到排序前的状态。 ");
        }

        /// <summary>
        /// 选择排序
        /// </summary>
        /// <param name="data"></param>
        private static IEnumerable<SortStep> SelectSort(IEnumerable<int> data)
        {
            count = 1;
            var array = data.ToArray();

            int min;
            for (var i = 0; i < array.Length - 1; i++)
            {
                yield return GetSortStep(StepCategory.MoveIndivator, 1, i, $"[{count++}]将指示器A移动至{i + 1}格。", i);
                min = i;//查找最小值
                for (var j = i + 1; j < array.Length; j++)
                {
                    if (array[min] > array[j])
                    {
                        min = j;//交换
                        yield return GetSortStep(StepCategory.MoveIndivator, 2, j, $"[{count++}]将指示器B移动至{i + 1}格。找到一个最小值{array[min]}", min);
                    }
                    else
                        yield return GetSortStep(StepCategory.MoveIndivator, 2, j, $"[{count++}]将指示器B移动至{i + 1}格。当前最小值{array[min]}");
                }
                if (min != i)
                {
                    var temp = array[min];
                    array[min] = array[i];
                    array[i] = temp;
                    yield return GetSortStep(StepCategory.SwapBar, min, i, $"[{count++}]交换扫描器A{ array[i] }与最小值{ array[min]}的位置。");
                }
            }
            yield return GetSortStep(StepCategory.Completed, 0, 0, $"排序已完成，共{count - 1}步。点击停止按钮将恢复到排序前的状态。 ", -2);
        }

        /// <summary>
        /// 冒泡排序
        /// </summary>
        /// <param name="data"></param>
        private static IEnumerable<SortStep> BubbleSort(IEnumerable<int> data)
        {
            count = 1;
            var array = data.ToArray();

            for (int i = 0; i < array.Length - 1; i++)
            {
                yield return GetSortStep(StepCategory.MoveIndivator, 1, i, $"[{count++}]将指示器A移动至{i + 1}格。");
                for (int j = 0; j < array.Length - 1 - i; j++)
                {
                    yield return GetSortStep(StepCategory.MoveIndivator, 2, j, $"[{count++}]将指示器B移动至{j + 1}格。", j);
                    if (array[j] > array[j + 1])
                    {
                        var temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                        yield return GetSortStep(StepCategory.SwapBar, j, j + 1, $"[{count++}]找到一个较大值{ array[j + 1] }，交换它与{ array[j]}的位置。", j + 1);
                    }
                }
            }
            yield return GetSortStep(StepCategory.Completed, 0, 0, $"排序已完成，共{count - 1}步。点击停止按钮将恢复到排序前的状态。 ");
        }

        /// <summary>
        /// 希尔排序
        /// </summary>
        private static IEnumerable<SortStep> ShellSort(IEnumerable<int> data)
        {
            count = 1;
            var array = data.ToArray();

            int gap;
            for (gap = array.Length / 2; gap > 0; gap /= 2)
            {
                for (var i = gap; i < array.Length; i++)
                {
                    yield return GetSortStep(StepCategory.MoveIndivator, 1, i, $"[{count++}]当前增量为{gap}，将指示器A移动至{i + 1}格。");
                    for (var j = i - gap; j >= 0; j -= gap)
                    {
                        yield return GetSortStep(StepCategory.MoveIndivator, 2, j, $"[{count++}]将指示器B移动至{j + 1}格。");
                        if (array[j] > array[j + gap])
                        {
                            var temp = array[j];
                            array[j] = array[j + gap];
                            array[j + gap] = temp;
                            yield return GetSortStep(StepCategory.SwapBar, j, j + gap, $"[{count++}]指示器B的值{ array[j] }比它之后第{gap}个值{array[j + gap]}更大的值，交换两者的位置。");
                        }
                        else
                            break;
                    }
                }
            }
            yield return GetSortStep(StepCategory.Completed, 0, 0, $"排序已完成，共{count - 1}步。点击停止按钮将恢复到排序前的状态。 ");

        }

        /// <summary>
        /// 快速排序
        /// </summary>
        private static IEnumerable<SortStep> QuickSort(IEnumerable<int> data)
        {
            var array = data.ToArray();
            count = 1;
            foreach (var item in QSort(array, 0, array.Length - 1))
                yield return item;
            yield return GetSortStep(StepCategory.Completed, 0, 0, $"排序已完成，共{count - 1}步。点击停止按钮将恢复到排序前的状态。 ");
        }
        static int count = 0;
        private static IEnumerable<SortStep> QSort(int[] array, int low, int high)
        {
            if (low >= high)
                yield break;

            int first = low;
            int last = high;
            var key = array[first];/*用字表的第一个记录作为枢轴*/

            yield return GetSortStep(StepCategory.MoveIndivator, 1, first, $"[{count++}]设置标杆为{key}，将指示器A移动至{first + 1}格。", first);

            yield return GetSortStep(StepCategory.MoveIndivator, 2, last, $"[{count++}]将指示器B移动至{last + 1}格。");


            while (first < last)
            {
                while (first < last && array[last] >= key)
                {
                    --last;
                    yield return GetSortStep(StepCategory.MoveIndivator, 2, last, $"[{count++}]将指示器B移动至{last + 1}格，寻找一个比标杆小的值。");

                }
                if (last == first)
                    break;

                yield return GetSortStep(StepCategory.SwapBar, last, first, $"[{count++}]找到一个较小值{ array[last] }，交换指示器所在的位置元素。");

                array[first] = array[last];/*将比第一个小的移到低端*/

                while (first < last && array[first] <= key)
                {
                    ++first;
                    yield return GetSortStep(StepCategory.MoveIndivator, 1, first, $"[{count++}]将指示器A移动至{first + 1}格，寻找一个比标杆大的值。");

                }
                if (last == first)
                    break;

                yield return GetSortStep(StepCategory.SwapBar, last, first, $"[{count++}]找到一个较大值{ array[first] }，交换指示器所在的位置元素。");


                array[last] = array[first];
                /*将比第一个大的移到高端*/
            }

            array[first] = key;/*枢轴记录到位*/
            foreach (var item in QSort(array, low, first - 1))
                yield return item;
            foreach (var item in QSort(array, first + 1, high))
                yield return item;
        }

        static int[] _indexList;
        /// <summary>
        /// 归并排序
        /// </summary>
        private static IEnumerable<SortStep> MergeSort(IEnumerable<int> data)
        {
            count = 1;
            _indexList = new int[data.Count()];
            for (int i = 0; i < data.Count(); i++)
                _indexList[i] = i;

            var array = data.ToArray();
            foreach (var item in MSort(array, 0, array.Length - 1))
                yield return item;
            yield return GetSortStep(StepCategory.Completed, 0, 0, $"排序已完成，共{count - 1}步。点击停止按钮将恢复到排序前的状态。 ");
        }
        private static IEnumerable<SortStep> MSort(int[] array, int first, int last)
        {
            if (first < last)
            {
                int mid = (first + last) / 2;
                foreach (var item in MSort(array, first, mid))
                    yield return item;
                foreach (var item in MSort(array, mid + 1, last))
                    yield return item;
                foreach (var item in MergeMethid(array, first, mid, last))
                    yield return item;
            }
        }
        private static IEnumerable<SortStep> MergeMethid(int[] array, int first, int mid, int last)
        {
            yield return GetSortStep(StepCategory.MoveIndivator, 1, first, $"[{count++}]将指示器A移动至{first + 1}格。");
            yield return GetSortStep(StepCategory.MoveIndivator, 2, last, $"[{count++}]将指示器B移动至{last + 1}格，对AB区间内的数组进行排序。");
            //记录索引变化
            List<int> indexList = new List<int>();
            int[] temp = new int[last - first + 1];

            //修改后的索引
            int[] index = new int[last + 1];

            int m = first, n = mid + 1, k = 0;
            while (n <= last && m <= mid)
            {
                if (array[m] > array[n])
                {
                    temp[k] = array[n];
                    yield return GetSortStep(StepCategory.MoveBar, n, first + k, $"[{count++}]将区间中的最小值{temp[k]}移动到临时区中。", n, 1);
                    index[k++] = _indexList[n++];
                }
                else
                {
                    temp[k] = array[m];
                    yield return GetSortStep(StepCategory.MoveBar, m, first + k, $"[{count++}]将区间中的最小值{temp[k]}移动到临时区中。", m, 1);
                    index[k++] = _indexList[m++];
                }
            }
            while (n < last + 1)
            {
                temp[k] = array[n];
                yield return GetSortStep(StepCategory.MoveBar, n, first + k, $"[{count++}]将区间中的最小值{temp[k]}移动到临时区中。", n, 1);
                index[k++] = _indexList[n++];
            }
            while (m < mid + 1)
            {
                temp[k] = array[m];
                yield return GetSortStep(StepCategory.MoveBar, m, first + k, $"[{count++}]将区间中的最小值{temp[k]}移动到临时区中。", m, 1);
                index[k++] = _indexList[m++];
            }
            for (int i = 0; i < last - first + 1; i++)
            {
                _indexList[i + first] = index[i];
            }
            for (k = 0, m = first; m < last + 1; k++, m++)
            {
                array[m] = temp[k];
                yield return GetSortStep(StepCategory.MoveBar, m, m, $"[{count++}]将临时区中的最小值{temp[k]}移回区间中。", m, -1);
            }
            yield return GetSortStep(_indexList, true);
        }
        /// <summary>
        /// 桶排序
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static IEnumerable<SortStep> BucketSort(IEnumerable<int> data)
        {
            count = 1;
            _indexList = new int[data.Count()];
            for (int i = 0; i < data.Count(); i++)
                _indexList[i] = i;

            var array = data.ToArray();
            //计算桶的数量和每个桶的大小（为了方便显示）
            var bucketQuantity = 0;
            int[] bucketVolumes;
            int delta = 0;
            CaculateBucket(data, out bucketQuantity, out bucketVolumes, out delta);
            yield return GetSortStep(bucketQuantity, bucketVolumes, $"[{count++}]生成{bucketQuantity}只桶。画布大小有限，不会被分配元素的桶将不会在画布中显示。");

            //创建桶并初始化
            var bucketArray = new ArrayList[bucketQuantity];
            for (int i = 0; i < bucketQuantity; i++)
                bucketArray[i] = new ArrayList();

            for (int i = 0; i < array.Count(); i++)
            {
                //进入的桶号
                var bucketNum = (int)Math.Ceiling((double)(array[i] / delta));
                bucketArray[bucketNum].Add(array[i]);
                //计算是桶内的元素总数
                var itemcount = 0;
                if (bucketNum != 0)
                    for (int j = 0; j < bucketNum; j++)
                        itemcount += bucketVolumes[j];
                var aimindex = itemcount + bucketArray[bucketNum].Count - 1;
                yield return GetSortStep(StepCategory.MoveBar, i, aimindex, $"[{count++}]将元素{array[i]}分配到{bucketNum + 1}桶中。", i, 1);
                _indexList[aimindex] = i;
            }
            yield return GetSortStep(_indexList, false, $"[{count++}]分配已完成。开始对桶依次执行插入排序。");
            //执行分排序
            for (int k = 0; k < bucketArray.Count(); k++)
            {
                if (bucketArray[k].Count == 0)
                    continue;
                var indexcount = 0;
                if (k != 0)
                    for (int j = 0; j < k; j++)
                        indexcount += bucketVolumes[j];
                int[] newarray = (int[])bucketArray[k].ToArray(typeof(int));

                //计算元素实际索引
                if (newarray.Length == 1)
                {
                    yield return GetSortStep(StepCategory.MoveIndivator, 1, indexcount, $"[{count++}]将指示器A移动至{indexcount + 1}格。");
                    yield return GetSortStep(StepCategory.MoveBar, indexcount, indexcount, $"[{count++}]将元素{newarray[0]}从桶中移回数组。", indexcount, -1);
                }
                else
                {
                    for (var i = 1; i < newarray.Length; i++)
                    {
                        yield return GetSortStep(StepCategory.MoveIndivator, 1, indexcount + i, $"[{count++}]将指示器A移动至桶{k + 1}内的第{i + 1}格。", -2);
                        for (var j = i; j > 0; j--)
                        {
                            yield return GetSortStep(StepCategory.MoveIndivator, 2, indexcount + j - 1, $"[{count++}]将指示器B移动至桶内的第{j}格，寻找比{ newarray[j] }更大的值");
                            if (newarray[j] < newarray[j - 1])
                            {
                                var temp = newarray[j];
                                newarray[j] = newarray[j - 1];
                                newarray[j - 1] = temp;
                                yield return GetSortStep(StepCategory.SwapBar, indexcount + j, indexcount + j - 1, $"[{count++}]找到一个较大值{ newarray[j - 1] }，交换它与{ newarray[j]}的位置。");
                            }
                            else
                                break;
                        }
                    }
                    for (int i = 0; i < newarray.Length; i++)
                    {
                        yield return GetSortStep(StepCategory.MoveBar, indexcount + i, indexcount + i, $"[{count++}]将元素{newarray[i]}从桶中移回数组。", indexcount + i, -1);
                    }
                }
            }
            yield return GetSortStep(StepCategory.Completed, 0, 0, $"排序已完成，共{count - 1}步。点击停止按钮将恢复到排序前的状态。 ");
        }
        private static void CaculateBucket(IEnumerable<int> data, out int bucketQuantity, out int[] bucketVolumes, out int delta)
        {
            bucketQuantity = data.Count();
            if (bucketQuantity > 10)
                bucketQuantity = 10;

            bucketVolumes = new int[bucketQuantity];
            delta = 10;
            var deltax = delta;
            for (int i = 0; i < bucketQuantity; i++)
            {
                bucketVolumes[i] = data.Count(x => (int)Math.Ceiling((double)(x / deltax)) == i);
            }

        }
        #region Function
        private static SortStep GetSortStep(StepCategory category, int from, int to, string description = "", int changeColor = -1, int raise = 0)
        {
            return new SortStep()
            {
                Category = category,
                From = from,
                To = to,
                Description = description,
                SelectedIndex = changeColor,
                Raise = raise,
            };
        }
        private static SortStep GetSortStep(int[] indexList, bool skipwaiting, string description = "")
        {
            return new SortStep()
            {
                Category = StepCategory.ChangeIndex,
                ChangedIndexList = indexList,
                Description = description,
                SkipWaiting = skipwaiting,
            };
        }
        private static SortStep GetSortStep(int bucketQuantity, int[] bucketVolumes, string description = "")
        {
            return new SortStep()
            {
                Category = StepCategory.CreateBucket,
                BucketQuantity = bucketQuantity,
                BucketVolumes = bucketVolumes,
                Description = description,
            };

        }
        #endregion
    }

}
