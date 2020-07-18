package Natasha;

import java.util.Random;
import java.util.concurrent.CountDownLatch;

public class CallableTask implements Runnable {
    private int columnNumber;
    Random r = new Random();
    int [] ColumnsSum;
    int[][] array;
    int N;//Y
    int M;//X
    CountDownLatch countDownLatch;

    public CallableTask(CountDownLatch countDownLatch, int columnNumber, int [][]array, int[] sumElements, int N, int M) {
        this.array=array;
        this.ColumnsSum = sumElements;
        this.N= N;
        this.M=M;
        this.columnNumber= columnNumber;
        this.countDownLatch = countDownLatch;
    }

    @Override
    public void run()  {
        int sum = 0;
        for (int i = 0; i < N;i ++) {
            int ran= r.nextInt(100);
//            array[i][columnNumber] = ran;
            array[columnNumber][i] = ran;

            sum+=ran;
        }
        ColumnsSum[columnNumber]= array[columnNumber][0];
        countDownLatch.countDown();

    }
}
