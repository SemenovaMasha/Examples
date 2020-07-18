package Natasha;

import java.util.Random;
import java.util.concurrent.*;

public class MainRecursive {
    public static void main(String[] args) {
        long time0 = System.currentTimeMillis();
        long endTime ;
        long startTime;

        Random random = new Random();
        int sizeX = random.nextInt(10)+1;
        int sizeY = random.nextInt(10)+1;
        sizeX= 10;
        sizeY=10;
        startTime= System.currentTimeMillis();
        int[][] array = new int[sizeY][sizeX];

        int[] arraySum = new int[sizeX];

        ExecutorService executorService = Executors.newFixedThreadPool(6);

        CountDownLatch countDownLatch = new CountDownLatch(sizeX);

        for(int i=0;i<sizeX;i++){
            executorService.submit(new CallableTask(countDownLatch, i, array, arraySum, sizeY, sizeX) );
        }
        try {
            countDownLatch.await();
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
//        executorService.shutdown();
        endTime = System.currentTimeMillis();
        System.out.println("Initialization Time: " + (endTime - startTime) );

//        for (int i = 0; i < array.length; i++) {
//            System.out.print(arraySum[i] + "\t");
//        }
        printArray(array);
        printArray(arraySum);

        startTime= System.currentTimeMillis();

//        MergeSort m = new MergeSort(arraySum, 0, arraySum.length);
//        m.invoke();

        ForkJoinPool commonPool = ForkJoinPool.commonPool();
        MergeSort m = new MergeSort(arraySum, 0, arraySum.length);
        commonPool.execute(m);
        commonPool.shutdown();
        try {
            commonPool.awaitTermination(Long.MAX_VALUE, TimeUnit.MILLISECONDS);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }

        endTime = System.currentTimeMillis();
        System.out.println("Sort Time: " + (endTime - startTime) );



        startTime= System.currentTimeMillis();
        int [][] newArray = new int [array.length][array[0].length];
        countDownLatch = new CountDownLatch(sizeX);

        for (int j = 0; j < m.orderedIndexes.length; j++) {
            executorService.submit( new PermuteCallable(j,m.orderedIndexes[j],array,newArray,countDownLatch));
//            copyColumn(array,m.orderedIndexes[j],newArray,j);
        }
        try {
            countDownLatch.await();
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
         endTime = System.currentTimeMillis();
        System.out.println("permute: " + (endTime - startTime) );

        executorService.shutdown();

        printArray(newArray);
        printArray(m.result);

        endTime = System.currentTimeMillis();
        System.out.println();
        System.out.println("Total Time: " + (endTime - time0) );


    }

    public static void printArray(int[][] array){
        for (int i = 0; i < array.length; i++) {
            for (int j = 0; j < array[i].length; j++) {
                System.out.print(String.format("%3s", array[i][j]));

            }
            System.out.println();
        }
    }
    public static void printArray(int[] array){
        for (int i = 0; i < array.length; i++) {
            System.out.print(array[i]+" ");
        }
        System.out.println();
    }


}
