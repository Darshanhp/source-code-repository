using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATRIX
{
    public class Matrix<T>
    {
        T[,] matRep;
        int rows_;
        int cols_;

        public Matrix(int i, int j)
        {
            rows_ = i; cols_ = j;
            matRep = new T[rows_, cols_];
        }
        public T this[int i, int j]
        {
            get { return matRep[i, j]; }
            set { matRep[i, j] = value; }
        }
        public void show()
        {
            for (int i = 0; i < rows_; i++)
            {
                Console.Write("\n");
                for (int j = 0; j < cols_; j++)
                    Console.Write(" {0}", this[i, j]);
            }
            Console.Write("\n");
        }
    }
    class program
    {
        static void Main(string[] args)
        {
            Matrix<double> mat = new Matrix<double>(3, 2);
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 2; j++)
                {
                    mat[i, j] = i * j + 3;
                }
            mat.show();
        }
    }
}