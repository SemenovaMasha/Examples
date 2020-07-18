package Futures;

import java.util.Random;
import java.util.concurrent.Callable;
import java.util.concurrent.CountDownLatch;

public class CallableForConcurrent implements Runnable {
    private int rowNumber;
    Random r = new Random();

    int [] maxElements;
    int[][] array;
    int N;
    int M;
    CountDownLatch countDownLatch;

    public CallableForConcurrent(CountDownLatch countDownLatch,int rowNumber, int [][]array, int[] maxElements, int N, int M) {
        this.array=array;
        this.maxElements = maxElements;
        this.N= N;
        this.M=M;
        this.rowNumber=rowNumber;
        this.countDownLatch = countDownLatch;
    }

    @Override
    public void run()  {
        int nextElement=r.nextInt(10000)-5000;
        array[rowNumber][0] = nextElement;
        maxElements[rowNumber]=nextElement;

        for (int j = 1; j < M; j++) {

            nextElement=r.nextInt(10000)-5000;
            array[rowNumber][j] = nextElement;

        }
        countDownLatch.countDown();

    }
}

