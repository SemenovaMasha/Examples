package Natasha;

import java.util.concurrent.ForkJoinPool;
import java.util.concurrent.RecursiveAction;

public class QuickSort {

    Comparable array[];
    static int limiter = 10000;

    public QuickSort(Comparable array[]) {
        this.array = array;
    }

    public void sort(ForkJoinPool pool) {
        RecursiveAction start = new Partition(0, array.length - 1);
        pool.invoke(start);
    }

    class Partition extends RecursiveAction {

        int left;
        int right;

        Partition(int left, int right) {
            this.left = left;
            this.right = right;
        }

        public int size() {
            return right - left;
        }

        @SuppressWarnings("empty-statement")
        protected void compute() {
            int i = left, j = right;
            Comparable tmp;
            Comparable pivot = array[(left + right) / 2];

            while (i <= j) {
                while (array[i].compareTo(pivot) < 0) {
                    i++;
                }
                while (array[j].compareTo(pivot) > 0) {
                    j--;
                }

                if (i <= j) {
                    tmp = array[i];
                    array[i] = array[j];
                    array[j] = tmp;
                    i++;
                    j--;
                }
            }


            Partition leftTask = null;
            Partition rightTask = null;

            if (left < i - 1) {
                leftTask = new Partition(left, i - 1);
            }
            if (i < right) {
                rightTask = new Partition(i, right);
            }

            if (size() > limiter) {
                if (leftTask != null && rightTask != null) {
                    invokeAll(leftTask, rightTask);
                } else if (leftTask != null) {
                    invokeAll(leftTask);
                } else if (rightTask != null) {
                    invokeAll(rightTask);
                }
            }else{
                if (leftTask != null) {
                    leftTask.compute();
                }
                if (rightTask != null) {
                    rightTask.compute();
                }
            }
        }
    }
}