package Natasha;

import java.util.Random;
import java.util.concurrent.CountDownLatch;

public class PermuteCallable  implements Runnable {
    private int columnIndexTarget;
    private int columnIndexSource;

    private int[][] arraySource;
    private int[][] arrayTarget;

    CountDownLatch countDownLatch;

    public PermuteCallable(int columnIndexTarget, int columnIndexSource, int[][] arraySource, int[][] arrayTarget, CountDownLatch countDownLatch) {
        this.columnIndexTarget = columnIndexTarget;
        this.columnIndexSource = columnIndexSource;
        this.arraySource = arraySource;
        this.arrayTarget = arrayTarget;

        this.countDownLatch = countDownLatch;
    }

    @Override
    public void run()  {
        for (int i = 0;i < arraySource.length; i++) {
//            arrayTarget[i][columnIndexTarget] = arraySource[i][columnIndexSource];
            arrayTarget[columnIndexTarget][i] = arraySource[columnIndexSource][i];
        }
        countDownLatch.countDown();

    }
}
