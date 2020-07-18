package Natasha;


import java.util.Arrays;
import java.util.Random;

public class Main {

    public static void main(String[] args) {
        Random r = new Random();
//        int[]a = new int []{17,2,4,2,4,6,1,3,12,5};
//        int[]a = new int [1000_000];
        int[]a = new int [10];

        long startTime = System.currentTimeMillis();

        for (int i = 0; i < a.length; i++) {
            a[i] = r.nextInt(1000);
            System.out.print(a[i] +" ");
        }
        System.out.println();
        long endTime = System.currentTimeMillis();
        System.out.println("Initialization: " + (endTime - startTime) );
        startTime = System.currentTimeMillis();

        MergeSort m = new MergeSort(a, 0, a.length);

        m.invoke();

        endTime = System.currentTimeMillis();
        System.out.println("Sort: " + (endTime - startTime) );
        startTime = System.currentTimeMillis();

        for (int i = 0; i < m.result.length; i++) {
            System.out.print(m.result[i] +" ");
        }
        System.out.println();
        for (int i = 0; i < m.orderedIndexes.length; i++) {
            System.out.print(m.orderedIndexes[i] +" ");
        }
        System.out.println();
        System.out.println();

        for (int i = 0; i < a.length; i++) {
            a[i] = r.nextInt(1000);
//            System.out.print(a[i] +" ");
        }
        System.out.println();
        endTime = System.currentTimeMillis();
        System.out.println("Initialization: " + (endTime - startTime) );
        startTime = System.currentTimeMillis();


        a = sortMerge(a);


        for (int i = 0; i < a.length; i++) {
            System.out.print(a[i] +" ");
        }

        endTime = System.currentTimeMillis();
        System.out.println();
        System.out.println("Sort: " + (endTime - startTime) );
        startTime = System.currentTimeMillis();


    }
    private static int[] sortMerge(int[] arr) {
        int len = arr.length;
        if (len < 2) return arr;
        int middle = len / 2;
        return merge(sortMerge(Arrays.copyOfRange(arr, 0, middle)),
                sortMerge(Arrays.copyOfRange(arr, middle, len)));
    }

    private static int[] merge(int[] arr_1, int[] arr_2) {
        int len_1 = arr_1.length, len_2 = arr_2.length;
        int a = 0, b = 0, len = len_1 + len_2; // a, b - счетчики в массивах
        int[] result = new int[len];
        for (int i = 0; i < len; i++) {
            if (b < len_2 && a < len_1) {
                if (arr_1[a] > arr_2[b]) result[i] = arr_2[b++];
                else result[i] = arr_1[a++];
            } else if (b < len_2) {
                result[i] = arr_2[b++];
            } else {
                result[i] = arr_1[a++];
            }
        }
        return result;
    }

}
