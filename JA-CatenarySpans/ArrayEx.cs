using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JA
{

    /// <summary>
    /// Code taken from http://msdn.microsoft.com/en-us/magazine/jj863137.aspx
    /// </summary>
    public static class ArrayEx
    {
        public static Random rnd=new Random();

        #region Factory
        public static double[] VectorCreate(int rows)
        {
            return new double[rows];
        }
        public static double[] VectorCreate(int rows, Func<int, double> init)
        {
            double[] result=new double[rows];
            for (int i=0; i<rows; ++i)
            {
                result[i]=init(i);
            }
            return result;
        }
        public static double[][] MatrixCreate(int rows, int cols)
        {
            // creates a matrix initialized to all 0.0s  
            // do error checking here?  
            double[][] result=new double[rows][];
            for (int i=0; i<rows; ++i)
                result[i]=new double[cols];
            // auto init to 0.0  
            return result;
        }
        public static double[][] MatrixCreate(int rows, int cols, Func<int, int, double> init)
        {
            // do error checking here?  
            double[][] result=new double[rows][];
            for (int i=0; i<rows; ++i)
            {
                var row=new double[cols];
                for (int j=0; j<cols; j++)
                {
                    row[j]=init(i, j);
                }
                result[i]=row;
            }
            return result;
        }
        public static double[][] MatrixCreateDiagonal(double[] values)
        {
            int rows=values.Length;
            int cols=values.Length;
            // do error checking here?  
            double[][] result=new double[rows][];
            for (int i=0; i<rows; ++i)
            {
                var row=new double[cols];
                row[i]=values[i];
                result[i]=row;
            }
            return result;
        }
        public static double[] VectorRandom(int rows,
          double minVal, double maxVal)
        {
            // return matrix with values between minVal and maxVal
            double[] result=new double[rows];
            for (int i=0; i<rows; ++i)
                result[i]=(maxVal-minVal)*rnd.NextDouble()+minVal;
            return result;
        }
        public static double[][] MatrixRandom(int rows, int cols,
          double minVal, double maxVal)
        {
            // return matrix with values between minVal and maxVal
            double[][] result=MatrixCreate(rows, cols);
            for (int i=0; i<rows; ++i)
                for (int j=0; j<cols; ++j)
                    result[i][j]=(maxVal-minVal)*rnd.NextDouble()+minVal;
            return result;
        }
        public static double[][] MatrixIdentity(int n)
        {
            double[][] result=MatrixCreate(n, n);
            for (int i=0; i<n; ++i)
                result[i][i]=1.0;
            return result;
        }
        public static double[] VectorDuplicate(this double[] vector)
        {
            // assumes matrix is not null.
            double[] result=new double[vector.Length];
            for (int i=0; i<result.Length; ++i) // copy the values
                result[i]=vector[i];
            return result;
        }
        public static double[][] MatrixDuplicate(this double[][] matrix)
        {
            // assumes matrix is not null.
            double[][] result=MatrixCreate(matrix.Length, matrix[0].Length);
            for (int i=0; i<matrix.Length; ++i) // copy the values
                for (int j=0; j<matrix[i].Length; ++j)
                    result[i][j]=matrix[i][j];
            return result;
        }

        #endregion

        #region Matrix Algebra and Operations
        public static bool VectorAreEqual(this double[] vectorA,
            double[] vectorB, double epsilon)
        {
            // true if all values in A == corresponding values in B
            int aRows=vectorA.Length;
            int bRows=vectorB.Length;
            if (aRows!=bRows) return false;
            for (int i=0; i<aRows; ++i) // each row of A and B
                if (Math.Abs(vectorA[i]-vectorB[i])>epsilon)
                    return false;
            return true;
        }
        public static bool MatrixAreEqual(this double[][] matrixA,
            double[][] matrixB, double epsilon)
        {
            // true if all values in A == corresponding values in B
            int aRows=matrixA.Length; int aCols=matrixA[0].Length;
            int bRows=matrixB.Length; int bCols=matrixB[0].Length;
            if (aRows!=bRows||aCols!=bCols) return false;
            for (int i=0; i<aRows; ++i) // each row of A and B
                for (int j=0; j<bCols; ++j) // each col of A and B
                    if (Math.Abs(matrixA[i][j]-matrixB[i][j])>epsilon)
                        return false;
            return true;
        }
        public static double[][] MatrixTranspose(this double[][] matrix)
        {
            int n=matrix.Length;
            int m=matrix[0].Length;

            double[][] trans=new double[m][];
            for (int i=0; i<m; i++)
            {
                var row=new double[n];
                for (int j=0; j<n; j++)
                {
                    row[j]=matrix[j][i];
                }
                trans[i]=row;
            }
            return trans;
        }

        public static double[] VectorScale(this double[] vectorA, double scale)
        {
            double[] result=new double[vectorA.Length];
            VectorScale(vectorA, result, scale);
            return result;
        }

        public static void VectorScale(this double[] vectorA, double[] result, double scale)
        {
            int aRows=vectorA.Length;
            int cRows=result.Length;
            if (aRows!=cRows)
                throw new Exception("Non-conformable vectors in VectorScale");
            for (int i=0; i<aRows; i++)
            {
                result[i]=scale*vectorA[i];
            }
        }

        public static double[][] MatrixScale(this double[][] matrixA,
            double factor)
        {
            int aRows=matrixA.Length; int aCols=matrixA[0].Length;
            double[][] matrixResult=MatrixCreate(aRows, aCols);

            MatrixScale(matrixA, matrixResult, factor);

            return matrixResult;
        }
        public static void MatrixScale(this double[][] matrixA,
            double[][] matrixResult, double factor)
        {
            int aRows=matrixA.Length; int aCols=matrixA[0].Length;
            int cRows=matrixResult.Length; int cCols=matrixResult[0].Length;
            if (aCols!=cCols||aRows!=cRows)
                throw new Exception("Non-conformable matrices in MatrixScale");
            for (int i=0; i<aRows; ++i) // each row of A
            {
                double[] Arow=matrixA[i];
                double[] Crow=matrixResult[i];
                for (int j=0; j<aCols; ++j) // each col of A
                    Crow[j]=factor*Arow[j];
            }
        }
        public static double[] VectorAddition(this double[] vectorA,
            double[] vectorB, double factorB)
        {
            double[] vectorResult=new double[Math.Max(vectorA.Length, vectorB.Length)];            
            VectorAddition(vectorA, vectorB, vectorResult, factorB);
            return vectorResult;
        }
        public static void VectorAddition(this double[] vectorA,
            double[] vectorB, double[] resultVector, double factorB)
        {

            if (vectorB.Length>vectorA.Length)
            {
                for (int i=0; i<vectorA.Length; i++)
                {
                    resultVector[i]=vectorA[i]+factorB*vectorB[i];
                }
                for (int i=vectorA.Length; i<vectorB.Length; i++)
                {
                    resultVector[i]=factorB*vectorB[i];
                }
            }
            else
            {
                for (int i=0; i<vectorB.Length; i++)
                {
                    resultVector[i]=vectorA[i]+factorB*vectorB[i];
                }
                for (int i=vectorB.Length; i<vectorA.Length; i++)
                {
                    resultVector[i]=vectorA[i];
                }
            }
        }
        public static double[][] MatrixAddition(this double[][] matrixA,
            double[][] matrixB, double factorB)
        {
            int aRows=matrixA.Length; int aCols=matrixA[0].Length;
            int bRows=matrixB.Length; int bCols=matrixB[0].Length;
            if (aCols!=bCols||aRows!=bRows)
                throw new Exception("Non-conformable matrices in MatrixAddition");
            double[][] matrixResult=MatrixCreate(aRows, aCols);

            MatrixAddition(matrixA, matrixB, matrixResult, factorB);

            return matrixResult;
        }
        public static void MatrixAddition(this double[][] matrixA,
            double[][] matrixB, double[][] matrixResult, double factorB)
        {
            int aRows=matrixA.Length; int aCols=matrixA[0].Length;
            int bRows=matrixB.Length; int bCols=matrixB[0].Length;
            int cRows=matrixResult.Length; int cCols=matrixResult[0].Length;
            if (aRows!=bRows||aRows!=cRows)
                throw new Exception("Non-conformable matrices in MatrixAddition");
            for (int i=0; i<aRows; ++i) // each row of A
            {
                VectorAddition(matrixA[i], matrixB[i], matrixResult[i], factorB);
            }
        }

        public static double[] VectorProduct(this double[] vectorA, double[] vectorB)
        {
            if (vectorA.Length!=vectorB.Length)
            {
                throw new Exception("Non-conformable vectors in InnerProduct");
            }
            double[] resultVector=new double[vectorA.Length];
            VectorProduct(vectorA, vectorB, resultVector);
            return resultVector;
        }

        public static void VectorProduct(this double[] vectorA, double[] vectorB, double[] resultVector)
        {
            int aRows=vectorA.Length;
            int bRows=vectorB.Length;
            int cRows=resultVector.Length;
            if (aRows!=bRows||aRows!=cRows)
                throw new Exception("Non-conformable vectors in VectorAddition");
            for (int i=0; i<aRows; ++i) // each row of A
            {
                resultVector[i]=vectorA[i]*vectorB[i];
            }
        }

        public static double[] MatrixProduct(this double[][] matrixA,
          double[] vectorB)
        {
            int aRows=matrixA.Length; int aCols=matrixA[0].Length;
            int bRows=vectorB.Length;
            if (aCols!=bRows)
                throw new Exception("Non-conformable matrices in MatrixProduct");
            double[] result=new double[aRows];
            for (int i=0; i<aRows; ++i) // each row of A
                for (int k=0; k<aCols; ++k)
                    result[i]+=matrixA[i][k]*vectorB[k];
            return result;
        }
        public static double[][] MatrixProduct(this double[][] matrixA,
          double[][] matrixB)
        {
            int aRows=matrixA.Length; int aCols=matrixA[0].Length;
            int bRows=matrixB.Length; int bCols=matrixB[0].Length;
            if (aCols!=bRows)
                throw new Exception("Non-conformable matrices in MatrixProduct");
            double[][] result=MatrixCreate(aRows, bCols);
            for (int i=0; i<aRows; ++i) // each row of A
                for (int j=0; j<bCols; ++j) // each col of B
                    for (int k=0; k<aCols; ++k)
                        result[i][j]+=matrixA[i][k]*matrixB[k][j];
            return result;
        }

        public static double InnerProduct(this double[] vectorA, double[] vectorB)
        {
            double dot=0;
            if (vectorA.Length!=vectorB.Length)
            {
                throw new Exception("Non-conformable vectors in InnerProduct");
            }
            for (int i=0; i<vectorA.Length; i++)
            {
                dot+=vectorA[i]*vectorB[i];
            }
            return dot;
        }
        public static double VectorNorm2(this double[] vector)
        {
            return Math.Sqrt(InnerProduct(vector, vector))/vector.Length;
        }

        public static double MatrixNorm2(this double[][] matrix)
        {
            double sum=0;
            for (int i=0; i<matrix.Length; i++)
            {
                double[] row=matrix[i];
                for (int j=0; j<row.Length; j++)
                {
                    sum+=row[j]*row[j]/(row.Length*row.Length);
                }
            }
            return Math.Sqrt(sum)/matrix.Length;
        }

        #endregion

        #region System of Equations
        public static double[][] MatrixDecompose(this double[][] matrix,
      out int[] perm, out int toggle)
        {
            // Doolittle LUP decomposition.
            // assumes matrix is square.
            int n=matrix.Length; // convenience
            double[][] result=MatrixDuplicate(matrix);
            perm=new int[n];
            for (int i=0; i<n; ++i) { perm[i]=i; }
            toggle=1;
            for (int j=0; j<n-1; ++j) // each column
            {
                double colMax=Math.Abs(result[j][j]); // largest val in col j
                int pRow=j;
                for (int i=j+1; i<n; ++i)
                {
                    if (result[i][j]>colMax)
                    {
                        colMax=result[i][j];
                        pRow=i;
                    }
                }
                if (pRow!=j) // swap rows
                {
                    double[] rowPtr=result[pRow];
                    result[pRow]=result[j];
                    result[j]=rowPtr;
                    int tmp=perm[pRow]; // and swap perm info
                    perm[pRow]=perm[j];
                    perm[j]=tmp;
                    toggle=-toggle; // row-swap toggle
                }
                if (Math.Abs(result[j][j])<DoubleEx.tol)
                    return null; // consider a throw
                for (int i=j+1; i<n; ++i)
                {
                    result[i][j]/=result[j][j];
                    for (int k=j+1; k<n; ++k)
                        result[i][k]-=result[i][j]*result[j][k];
                }
            } // main j column loop
            return result;
        }
        public static double[][] MatrixInverse(this double[][] matrix)
        {
            int n=matrix.Length;
            double[][] result=MatrixDuplicate(matrix);
            double[][] lum = MatrixDecompose(matrix, out int[] perm, out int toggle);
            if (lum==null)
                throw new Exception("Unable to compute inverse");
            double[] b=new double[n];
            for (int i=0; i<n; ++i)
            {
                for (int j=0; j<n; ++j)
                {
                    if (i==perm[j])
                        b[j]=1.0;
                    else
                        b[j]=0.0;
                }
                double[] x=HelperSolve(lum, b);
                for (int j=0; j<n; ++j)
                    result[j][i]=x[j];
            }
            return result;
        }
        public static double MatrixDeterminant(this double[][] matrix)
        {
            double[][] lum = MatrixDecompose(matrix, out int[] perm, out int toggle);
            if (lum==null)
                throw new Exception("Unable to compute MatrixDeterminant");
            double result=toggle;
            for (int i=0; i<lum.Length; ++i)
                result*=lum[i][i];
            return result;
        }
        public static double[] SystemSolve(this double[][] A, double[] b)
        {
            // Solve Ax = b
            int n=A.Length;
            double[][] luMatrix = MatrixDecompose(A,
              out int[] perm, out int toggle);
            if (luMatrix==null)
                return null; // or throw
            double[] bp=VectorPermute(b, perm);
            //double[] bp=new double[b.Length]; //isn't b.Length == n ?
            //for (int i=0; i<n; ++i)
            //    bp[i]=b[perm[i]];
            double[] x=HelperSolve(luMatrix, bp);
            return x;
        }
        public static double[][] SystemSolve(this double[][] A, double[][] b)
        {
            // Solve A[n,n] x[n,m] = b[n,m]
            int n=A.Length;
            int m=b[0].Length;
            double[][] luMatrix = MatrixDecompose(A,
              out int[] perm, out int toggle);
            if (luMatrix==null)
                return null; // or throw
            double[][] x=new double[m][];
            double[][] bp=MatrixTransposeAndPermute(b, perm);
            for (int j=0; j<m; j++)
            {
                x[j]=HelperSolve(luMatrix, bp[j]);
            }
            return MatrixTranspose(x);
        }
        #endregion

        #region Helper Functions
        static double[] VectorPermute(double[] vector, int[] row_perm)
        {
            double[] result=new double[vector.Length];
            for (int i=0; i<result.Length; i++)
            {
                result[i]=vector[row_perm[i]];
            }
            return result;
        }

        static double[][] MatrixTransposeAndPermute(this double[][] matrix, int[] row_perm)
        {
            int n=matrix.Length;
            int m=matrix[0].Length;

            double[][] trans=new double[m][];
            for (int i=0; i<m; i++)
            {
                var row=new double[n];
                for (int j=0; j<n; j++)
                {
                    row[j]=matrix[row_perm[j]][i];
                }
                trans[i]=row;
            }
            return trans;
        }

        static double[] HelperSolve(double[][] luMatrix,
    double[] b)
        {
            // solve luMatrix * x = b
            int n=luMatrix.Length;
            double[] x=new double[n];
            b.CopyTo(x, 0);
            for (int i=1; i<n; ++i)
            {
                double sum=x[i];
                for (int j=0; j<i; ++j)
                    sum-=luMatrix[i][j]*x[j];
                x[i]=sum;
            }
            x[n-1]/=luMatrix[n-1][n-1];
            for (int i=n-2; i>=0; --i)
            {
                double sum=x[i];
                for (int j=i+1; j<n; ++j)
                    sum-=luMatrix[i][j]*x[j];
                x[i]=sum/luMatrix[i][i];
            }
            return x;
        }
        static double[][] PermArrayToMatrix(int[] perm)
        {
            // Doolittle perm array to corresponding matrix
            int n=perm.Length;
            double[][] result=MatrixCreate(n, n);
            for (int i=0; i<n; ++i)
                result[i][perm[i]]=1.0;
            return result;
        }
        static double[][] UnPermute(double[][] luProduct, int[] perm)
        {
            double[][] result=MatrixDuplicate(luProduct);
            int[] unperm=new int[perm.Length];
            for (int i=0; i<perm.Length; ++i)
                unperm[perm[i]]=i; // create un-perm array
            for (int r=0; r<luProduct.Length; ++r) // each row
                result[r]=luProduct[unperm[r]];
            return result;
        }

        #endregion

    }

}