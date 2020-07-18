package Futures;

import java.util.Random;
import java.util.concurrent.*;

public class MainConcurrent {

    public static void main(String[] args) {
        int [] maxElements;
        int[][] array;
        int N;
        int M;
        long time0 = System.currentTimeMillis();
        long startTime = System.currentTimeMillis();
        long endTime ;
        Random random = new Random();
        N = random.nextInt(2)+5;
        M = random.nextInt(2)+5;
        N=10;
        M=10;

        int threadCount=6;

        System.out.println("ThreadCount: "+threadCount);

        array = new int[N][M];
        maxElements = new int[N];

        endTime = System.currentTimeMillis();
        System.out.println("Initialization: " + (endTime - startTime) );
        startTime = System.currentTimeMillis();

        ExecutorService executorService = Executors.newFixedThreadPool(threadCount);
        CountDownLatch countDownLatch= new CountDownLatch(N);
        for(int i=0;i<N;i++){
            executorService.submit(new CallableForConcurrent(countDownLatch,i,array,maxElements,N,M));
        }
        try {
            countDownLatch.await();
        } catch (InterruptedException e) {
            e.printStackTrace();
        }

        executorService.shutdown();

        printArray(array,maxElements);

        endTime = System.currentTimeMillis();
        System.out.println("Generation and searching max elements: " + (endTime - startTime) );
        startTime = System.currentTimeMillis();

        int kk=0;
        for(int k=0;k<N-1;k++){
            int maxValue = maxElements[k];
            int maxIndex = k;
            for(int i=k+1;i<N;i++){
                if(maxElements[i]<maxValue){
                    maxValue=maxElements[i];
                    maxIndex=i;
                }
            }
            if(maxIndex!=k){
//                System.out.println(k+" "+maxValue+" "+maxIndex);
                int tmp = maxElements[k];
                maxElements[k]=maxElements[maxIndex];
                maxElements[maxIndex]=tmp;
                kk++;
                swapRows(array,k,maxIndex);
            }
        }

        endTime = System.currentTimeMillis();
        System.out.println("Sort Time: " + (endTime - startTime) );
        System.out.println("Total Time: " + (endTime - time0) );

        System.out.println();
        printArray(array,maxElements);
    }
    public static void swapRows(int[][] array,int row1,int row2){
        for (int j = 0; j < array[0].length; j++) {
            int tmp = array[row1][j];
            array[row1][j] = array[row2][j];
            array[row2][j]=tmp;
        }
//        int[] tmp = array[row1];
//        array[row1] = array[row2];
//        array[row2]=tmp;
    }

    public static void swapColumns(int[][] array,int col1,int col2){
        for (int i = 0;i < array.length; i++) {
            int tmp = array[i][col1];
            array[i][col1] = array[i][col2];
            array[i][col2]=tmp;
        }
    }

    public static void printArray(int[][] array){
        for (int i = 0; i < array.length; i++) {
            for (int j = 0; j < array[i].length; j++) {
                System.out.print(array[i][j]+" ");
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

    public static void printArray(int[][] array,int[] max_array){
        for (int i = 0; i < array.length; i++) {
            System.out.print(max_array[i]+" : ");
            for (int j = 0; j < array[i].length; j++) {
                System.out.print(array[i][j]+" ");
            }
            System.out.println();
        }
    }


}