package Callable;

import ThreadsSemaphores.Main;

import java.util.Random;
import java.util.concurrent.Callable;
import java.util.concurrent.Semaphore;

public class GenerateAndFindMaxElementRowsCallable implements Callable<Object> {
    private int startRowIndex;
    private int endRowIndex;
    Random r = new Random();

    int [] maxElements;
    int[][] array;
    int N;
    int M;

    public GenerateAndFindMaxElementRowsCallable( int startRowIndex, int countRows, int [][]array, int[] maxElements, int N, int M) {
        this.startRowIndex = startRowIndex;
        this.endRowIndex = (startRowIndex+countRows>= N)?N:startRowIndex+countRows;
        this.array=array;
        this.maxElements = maxElements;
        this.N= N;
        this.M=M;
    }

    @Override
    public Object call() throws Exception {
        for (int i = startRowIndex; i < endRowIndex; i++) {
            int nextElement=r.nextInt(100)-50;
            array[i][0] = nextElement;
            maxElements[i]=nextElement;
        }
        for (int i = startRowIndex; i < endRowIndex; i++) {
            for (int j = 1; j < M; j++) {

                int nextElement=r.nextInt(100)-50;
                array[i][j] = nextElement;

                if(nextElement>maxElements[i])
                    maxElements[i]=nextElement;
            }
        }


        return null;
    }
}