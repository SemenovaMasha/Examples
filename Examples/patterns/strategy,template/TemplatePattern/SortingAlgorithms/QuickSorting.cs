using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TemplatePattern.SortingAlgorithms
{
    public class QuickSorting : AbstractSortingClass
    {
        protected override string AlgorithmName => "Quick sort";

        public override void Sort()
        {
            QuicksortParallel(Array, 0, N - 1);
        }

        public void QuicksortSequential(int[] arr, int left, int right)
        {
            if (right > left)
            {
                int pivot = Partition(arr, left, right);
                QuicksortSequential(arr, left, pivot - 1);
                QuicksortSequential(arr, pivot + 1, right);
            }
        }

        public void QuicksortParallel(int[] arr, int left, int right)

        {
            const int SEQUENTIAL_THRESHOLD = 2048;
            if (right > left)
            {
                if (right - left < SEQUENTIAL_THRESHOLD)
                {
                    QuicksortSequential(arr, left, right);
                }
                else
                {
                    int pivot = Partition(arr, left, right);
                    Parallel.Invoke(new Action[] { delegate {QuicksortParallel(arr, left, pivot - 1);},
                                               delegate {QuicksortParallel(arr, pivot + 1, right);}
                });
                }
            }
        }

        public void Swap(int[] arr, int i, int j)
        {
            int tmp = arr[i];
            arr[i] = arr[j];
            arr[j] = tmp;
        }

        public int Partition(int[] arr, int low, int high)
        {
            int pivotPos = (high + low) / 2;
            int pivot = arr[pivotPos];
            Swap(arr, low, pivotPos);

            int left = low;
            for (int i = low + 1; i <= high; i++)
            {
                if (arr[i].CompareTo(pivot) < 0)
                {
                    left++;
                    Swap(arr, i, left);
                }
            }

            Swap(arr, low, left);
            return left;
        }
    }
}
