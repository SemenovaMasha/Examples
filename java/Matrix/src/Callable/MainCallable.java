package Callable;

import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import java.util.concurrent.*;

public class MainCallable {

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
        N=20000;
        M=20000;

        int threadCount=N;
        if(N>10&&N<1000)
            threadCount=10;
        else if(N>=1000) threadCount=100;

        System.out.println("ThreadCount: "+threadCount);

        int countRows=(int)((double)N/threadCount);
        int ost = N%threadCount;
        int startRow = 0;

        array = new int[N][M];
        maxElements = new int[N];

        ExecutorService executor = Executors.newFixedThreadPool(threadCount);

        endTime = System.currentTimeMillis();
        System.out.println("Initialization: " + (endTime - startTime) );
        startTime = System.currentTimeMillis();

        List<GenerateAndFindMaxElementRowsCallable> tasks = new ArrayList(threadCount);
        for (int i = 0; i < threadCount; i++) {
            int countForThread=i%threadCount<ost?countRows+1:countRows;

            tasks.add(new GenerateAndFindMaxElementRowsCallable( startRow, i%threadCount<ost?countRows+1:countRows,array,maxElements,N,M));
            startRow+=countForThread;

        }
        try {

            executor.invokeAll(tasks);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }

        executor.shutdown();

        endTime = System.currentTimeMillis();
        System.out.println("Generation and searching max elements: " + (endTime - startTime) );
        startTime = System.currentTimeMillis();


        for(int k=0;k<N-1;k++){
            int maxValue = maxElements[k];
            int maxIndex = k;
            for(int i=k+1;i<N;i++){
                if(maxElements[i]>maxValue){
                    maxValue=maxElements[i];
                    maxIndex=i;
                }
            }
            if(maxIndex!=k){
                int tmp = maxElements[k];
                maxElements[k]=maxElements[maxIndex];
                maxElements[maxIndex]=tmp;
                swapRows(array,k,maxIndex);
            }
        }

        endTime = System.currentTimeMillis();
        System.out.println("Sort Time: " + (endTime - startTime) );
        System.out.println("Total Time: " + (endTime - time0) );

        System.out.println();

//        printArray(array,maxElements);
    }
    public static void swapRows(int[][] array,int row1,int row2){
        for (int j = 0; j < array[0].length; j++) {
            int tmp = array[row1][j];
            array[row1][j] = array[row2][j];
            array[row2][j]=tmp;
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