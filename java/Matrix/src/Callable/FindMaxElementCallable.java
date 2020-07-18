package Callable;

import java.util.concurrent.Callable;
import java.util.concurrent.Semaphore;

public class FindMaxElementCallable implements Callable<Object> {
    private int startRowIndex;
    private int endRowIndex;

    public FindMaxElementCallable( int startRowIndex, int countRows) {
        this.startRowIndex = startRowIndex;
        this.endRowIndex = (startRowIndex+countRows>= MainCallable.N)? MainCallable.N:startRowIndex+countRows;

//        System.out.println(startRowIndex+" : "+countRows+" : "+endRowIndex);
    }

    @Override
    public Object call() throws Exception {
        for (int i = startRowIndex; i < endRowIndex; i++) {
            MainCallable.maxElements[i]= MainCallable.array[i][0];
        }
        for (int i = startRowIndex; i < endRowIndex; i++) {
//            System.out.println(Name+" : "+i);
            for (int j = 1; j < MainCallable.M; j++) {

                if(MainCallable.array[i][j]> MainCallable.maxElements[i])
                    MainCallable.maxElements[i]= MainCallable.array[i][j];
            }
        }



        return null;
    }
}