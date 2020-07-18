package Natasha;

import java.util.Arrays;
import java.util.concurrent.ForkJoinTask;
import java.util.concurrent.RecursiveAction;

public class MergeSort extends RecursiveAction {
    final int[] numbers;
    final int startPos, endPos;
    int[] result=null;
    int[] orderedIndexes=null;

    public MergeSort(int[] numbers,int start, int end) {
        this.numbers = numbers;
        endPos = end;
        startPos = start;

        result =  new int[size()];
        orderedIndexes =  new int[size()];
    }

    private void merge(MergeSort left, MergeSort right) {
        int i=0, leftPos=0, rightPos=0, leftSize = left.size(), rightSize = right.size();
        while (leftPos < leftSize && rightPos < rightSize) {

            if(left.result[leftPos] >= right.result[rightPos]){
                result[i]=left.result[leftPos];

                orderedIndexes[i] = left.orderedIndexes[leftPos];

                i++; leftPos++;
            }else{
                result[i]=right.result[rightPos];

                orderedIndexes[i] = right.orderedIndexes[rightPos];

                i++; rightPos++;
            }

//            result[i++] = (left.result[leftPos] <= right.result[rightPos])
//                    ? left.result[leftPos++]
//                    : right.result[rightPos++];

        }
        i = Pog(left, i, leftPos, leftSize);
        i = Pog(right, i, rightPos, rightSize);
    }

    private int Pog(MergeSort left, int i, int leftPos, int leftSize) {
        while (leftPos < leftSize) {
            result[i] = left.result[leftPos];
            orderedIndexes[i] = left.orderedIndexes[leftPos];
            i++; leftPos++;
        }
        return i;
    }


    public int size() {
        return endPos-startPos;
    }

    protected void compute() {
        if (size() < 2) {
            System.arraycopy(numbers, startPos, result, 0, size());

            orderedIndexes[0]= startPos;
//            Arrays.sort(result, 0, size());
        } else {
            int midpoint = size() / 2;
            MergeSort left = new MergeSort(numbers, startPos, startPos+midpoint);
            MergeSort right = new MergeSort(numbers, startPos+midpoint, endPos);

//            RecursiveAction.invokeAll(left, right);

            ForkJoinTask.invokeAll(left,right);

            merge(left, right);
        }
    }
}