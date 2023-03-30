

public class LatinSquare
{
    public int genericCounter;
    public DateTime startTime;
    public StreamWriter writer;
    List<int[][]>? validSquare;

    public LatinSquare()
    {
        using (writer = new StreamWriter(File.Create("n=7.txt"))) Part_1(7);
        //I could have done this with a loop but I think it's nice to look at
        /*using (writer = new StreamWriter(File.Create("n=3.txt"))) Part_1(3);
        using (writer = new StreamWriter(File.Create("n=4.txt"))) Part_1(4);
        using (writer = new StreamWriter(File.Create("n=5.txt"))) Part_1(5);
        using (writer = new StreamWriter(File.Create("n=6.txt"))) Part_1(6);
        using (writer = new StreamWriter(File.Create("P2_n=9.txt"))) Part_2(9);
        using (writer = new StreamWriter(File.Create("P2_n=10.txt"))) Part_2(10);
        using (writer = new StreamWriter(File.Create("P2_n=11.txt"))) Part_2(11);
        using (writer = new StreamWriter(File.Create("P2_n=12.txt"))) Part_2(12);
        using (writer = new StreamWriter(File.Create("P2_n=13.txt"))) Part_2(13);
        using (writer = new StreamWriter(File.Create("P2_n=14.txt"))) Part_2(14);
        using (writer = new StreamWriter(File.Create("P2_n=15.txt"))) Part_2(15);
       */
    }

    
    #region PART 1 SPECIFIC
    public void Part_1(int size)
    {
        writer.WriteLine("Running Algorithm with size " + size);
        writer.WriteLine();

        //genericCounter is used for running the program on n=7 and above, so you know how far in you are. It's referenced right after a valid square is found
        genericCounter = 1;
        validSquare = new List<int[][]>();

        //For time tracking purposes. I couldn't get a way to record CPU time per run, but I proved some screenshots of executions in the accompanying document.
        //If you want to get the specifics (and if you're using visual studio) then commenting out all the undesired part 1/all part2 lines and hitting alt + f2 will give a value eventually.
        startTime = DateTime.Now;

        //Starts off the program
        Backtrack(EmptySquare(size), 1, 1);

        //Cleanup
        writer.WriteLine("Process finished in: " + (DateTime.Now - startTime).TotalMilliseconds + " ms");
        writer.WriteLine("Number of valid squares: " + validSquare.Count);
        writer.WriteLine("Examples: ");
        writer.WriteLine();

        //printing squares
        int i, max = 4;
        if (validSquare.Count < 4 || size == 5) max = validSquare.Count;
        for (i = 0; i < max; i++)
        {
            PrintSquare(validSquare[i]);
        }

    }

    //It permutes the square by sorting one of the rows. Then, it sorts it so that the leftmost column is in increasing order.
    //Elaborated upon in the doc.
    public int[][] ValidationSort(int[][] square, int row)
    {
        int size = square.Length;
        int[][] column_sorted = EmptySquare(square.Length);
        int[][] full_sorted = EmptySquare(square.Length);
        int i, j, k;

        //Sorting rows such that the "row"-th row is in increasing order
        for (i = 0; i < size; i++)
        {
            j = square[i][row] - 1;
            column_sorted[j] = square[i];
        }

        //Sorting by rows so that the leftmost column is in increasing order
        for (i = 0; i < size; i++)
        {
            k = column_sorted[0][i] - 1;
            for (j = 0; j < size; j++)
            {
                full_sorted[j][k] = column_sorted[j][i];
            }
        }
        return full_sorted;
    }

    //The backtracking algorithm for the assignment.
    public void Backtrack(int[][] square, int x, int y)
    {
        int i, tx = x, ty = y;
        //this iterator is for values that actually go into the table, so i is one of [1...n]
        for (i = 1; i <= square.Length; i++)
        {
            //if value i is valid at x,y then it is placed there and a new branch is made
            if (PositionIsValid(square, x, y, i))
            {
                //checks to see if the square is finished, or if the x and y coordinates are at their max. 
                square[x][y] = i;
                if (x == y && x == square.Length - 1)
                {

                    //check against old squares using the sorting method, then adds the square if it's inequivalent.
                    if (!SquareExists(square))
                    {
                        validSquare.Add(CopySquare(square));
                        //this is a means to keep track of how many inequivalent squares have been found so far in batches of 1000.
                        if (validSquare.Count / 1000 == genericCounter)
                        {
                            Console.WriteLine(genericCounter + "000 squares so far");
                            Console.WriteLine();
                            genericCounter++;
                        }
                    }
                }
                else //if the square's not done
                {
                    //making sure the y value doesn't wrap around
                    if (y + 1 == square.Length)
                    {
                        ty = 0;
                        tx += 1;

                    }
                    Backtrack(square, tx, ty + 1);
                }
                //voids the previously placed square so the process can repeat for every possible i
                square[x][y] = 0;
            }
        }
    }

    //Checking to make sure if the square exists. Elaborated upon in the doc.
    public bool SquareExists(int[][] square)
    {
        int i;
        int[][] temp;
        List<int[][]> permutations = new List<int[][]>();
        //This ensures that any new square is checked with all of its possible transformations.
        for (i = 0; i < square.Length; i++)
        {
            temp = ValidationSort(square, i);
            permutations.Add(temp);
        }
        //Checking if any permutation of the inequivalency candidate exists elsewhere.
        foreach (int[][] p in validSquare)
        {
            foreach (int[][] q in permutations)
            {
                if (IsEquivalent(p, q)) return true;
            }
        }
        return false;
    }

    //Checks the equivalency of two squares. Elaborated upon in the doc.
    public bool IsEquivalent(int[][] squareOne, int[][] squareTwo)
    {
        int i, j;
        for (i = 1; i < squareOne.Length - 1; i++)
        {
            for (j = 1; j < squareOne.Length - 1; j++)
            {
                if (squareOne[i][j] != squareTwo[i][j]) return false;
            }
        }


        return true;
    }
    #endregion

    #region PART 2 SPECIFIC
    public void Part_2(int size)
    {
        //used doubles for these values as they can get absolutely massive with n = [9..16]
        double nodeSum = 0d, nodeAverage;
        int[] nodesPerLevel;
        //arbitrary, but > 5
        int runCount = 10;
        //runs the test x times,, then calculates the average and prints it
        for (int i = 0; i < runCount; i++)
        {
            nodesPerLevel = BacktrackEst(EmptySquare(size), 1, 1, 0, new int[size * size]);
            nodeSum += NodeScale(nodesPerLevel);
        }

        nodeAverage = nodeSum / runCount;
        writer.WriteLine("Average over " + runCount + " runs: " + nodeAverage);
    }

    //NodeScale takes in an array of nodes by depth, then prints out what I'm meant to submit. Returns the full amount of estimated nodes in the tree.
    public double NodeScale(int[] estData)
    {
        writer.WriteLine("Depth:\t" + "# of Nodes:\t" + "Total # of estimated nodes:");
        int i = 0;
        double nodeCount = 1d;
        while (estData[i] != 0)
        {
            nodeCount *= estData[i];
            writer.WriteLine(i + "\t" + estData[i] + "\t\t" + nodeCount);
            i++;
        }
        writer.WriteLine("\nTotal: " + nodeCount + "\n");

        return nodeCount;
    }

    //The bactrack estimation function. Returns an array carrying the depth/node values
    //It works similar to the backtrack algorithm in part 1.
    public int[] BacktrackEst(int[][] square, int x, int y, int currentDepth, int[] nodesPerLevel)
    {
        Random r = new Random(DateTime.Now.Millisecond);
        int i, placedValue;
        int[] test = nodesPerLevel;
        List<int> validSelections = new List<int>();
        //if it actually managed to make a valid square, return early to prevent issues.
        if (x == y && x == square.Length - 1) return nodesPerLevel;
        //getting the possible moves
        for (i = 1; i <= square.Length; i++)
        {
            {
                if (PositionIsValid(square, x, y, i)) validSelections.Add(i);
                test[currentDepth]++;
            }

        }
        nodesPerLevel[currentDepth] = validSelections.Count;
        //if there are no valid options, dodge.
        if (validSelections.Count == 0) return test;

        //random selection, then continuation. Since it's meant to be an estimation algorithm based on the one we used for part 1, I redid the same style of picking the next cell "in line".
        //I could have run it on every possible placement beyond the x/y I already have (like putting down a 4 in the bottom right corner) but this reflects my actual implementation.
        placedValue = validSelections[r.Next(0, validSelections.Count)];
        square[x][y] = placedValue;
        if (y + 1 == square.Length)
        {
            y = 0;
            x += 1;
        }
        //recurse
        return BacktrackEst(square, x, y + 1, currentDepth + 1, test);
    }

    #endregion

    #region GENERAL UTILITY
    public void PrintSquare(int[][] square)
    {
        int i, j;
        for (i = 0; i < square.Length; i++)
        {
            for (j = 0; j < square.Length; j++)
            {
                writer.Write(square[j][i]);
            }
            writer.WriteLine();
        }
        writer.WriteLine();
    }

    //Empty is a vestigial name, it creates a new 2d array with the top row and left column in natural order. Elaborated upon in the doc.
    public int[][] EmptySquare(int size)
    {
        int[][] nSquare = new int[size][];
        int i;
        for (i = 0; i < size; i++)
        {
            nSquare[i] = new int[size];
        }
        for (i = 0; i < size; i++)
        {
            nSquare[0][i] = i + 1;
            nSquare[i][0] = i + 1;
        }

        return nSquare;
    }

    public int[][] CopySquare(int[][] square)
    {
        int i, j;
        int[][] temp = EmptySquare(square.Length);
        for (i = 1; i < square.Length; i++)
        {
            for (j = 1; j < square.Length; j++)
            {
                temp[i][j] = square[i][j];
            }
        }
        return temp;
    }

    //checks if a given placement with a given value is valid. Searches a row or column
    public bool PositionIsValid(int[][] square, int x, int y, int n)
    {
        int i;
        for (i = 0; i < square[x].Length; i++)
        {
            if (square[i][y] == n) return false; //searching across
        }

        if (square[x].Contains(n)) return false; //searching down
        return true;
    }

    #endregion

    public static void Main(string[] args)
    {
        LatinSquare l = new LatinSquare();
    }
}