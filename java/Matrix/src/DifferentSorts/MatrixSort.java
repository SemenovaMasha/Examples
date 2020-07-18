package Natasha;

import java.util.Random;
import java.util.concurrent.Callable;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class MatrixSort {

    public static void main(String[] args) {
        long time0 = System.currentTimeMillis();
        long endTime ;

        Random random = new Random();
        int sizeX = random.nextInt(10)+1;
        int sizeY = random.nextInt(10)+1;
        sizeX=2000;
        sizeY=2000;
        int[][] array = new int[sizeY][sizeX];

        int[] arraySum = new int[array[0].length];

        ExecutorService executorService = Executors.newFixedThreadPool(4);

        CountDownLatch countDownLatch = new CountDownLatch(sizeX);

        for(int i=0;i<sizeX;i++){
            executorService.submit(new CallableTask(countDownLatch, i, array, arraySum, sizeY, sizeX) );
        }
        try {
            countDownLatch.await();
        } catch (InterruptedException e) {
            e.printStackTrace();
        }

        executorService.shutdown();
//        for (int i = 0; i < array.length; i++) {
//            System.out.print(arraySum[i] + "\t");
//        }



        sort(array,arraySum);
        System.out.println();
//        System.out.print("Sort");
//        System.out.println();
//        for (int i = 0; i < array.length; i++) {
//                System.out.print(arraySum[i] + "\t");
//        }

        endTime = System.currentTimeMillis();
        System.out.println("Total Time: " + (endTime - time0) );


    }


    public static void sort(int[][] array,int[] arraySum) {
        long time0 = System.currentTimeMillis();
        long endTime ;

        for (int k = 0; k < arraySum.length - 1; k++) {
            for (int j = 0; j < arraySum.length - 1; j++) {
                if (arraySum[j + 1] > arraySum[j]) {
                    arraySum[j + 1] += arraySum[j] - (arraySum[j] = arraySum[j + 1]);
                    swapWithNextCol(array, j);
                }
            }
        }
        endTime = System.currentTimeMillis();
        System.out.println("Sort Time: " + (endTime - time0) );
    }

    public static void print(int[][] array) {
        for (int i = 0; i < array.length; i++) {
            for (int j = 0; j < array[0].length; j++) {
                System.out.print(array[i][j] + "\t");
            }
            System.out.println();
        }
    }

    public static void swapWithNextCol(int[][] array, int colNumber) { // меняет два столбца местамми
        for (int i = 0; i < array.length; i++) {
            array[i][colNumber + 1] += array[i][colNumber] - (array[i][colNumber] = array[i][colNumber + 1]);
        }
    }
}
