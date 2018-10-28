using System;
using System.Numerics;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using System.Collections.Generic;

namespace DigitalMusicAnalysis {
    public class timefreq {
        public float[][] timeFreqData;
        public int wSamp;
        public Complex[] twiddles;

        public timefreq(float[] x, int windowSamp) {
            int ii;
            double pi = 3.14159265;
            Complex i = Complex.ImaginaryOne;
            this.wSamp = windowSamp;
            twiddles = new Complex[wSamp];
            for (ii = 0; ii < wSamp; ii++) {
                double a = 2 * pi * ii / (double)wSamp;
                twiddles[ii] = Complex.Pow(Complex.Exp(-i), (float)a);
            }

            timeFreqData = new float[wSamp / 2][];

            int nearest = (int)Math.Ceiling((double)x.Length / (double)wSamp);
            nearest = nearest * wSamp;
            Complex[] compX = new Complex[nearest];
            for (int kk = 0; kk < nearest; kk++) {
                if (kk < x.Length) {
                    compX[kk] = x[kk];
                } else {
                    compX[kk] = Complex.Zero;
                }
            }
            
            int cols = 2 * nearest / wSamp;
            for (int jj = 0; jj < wSamp / 2; jj++) {
                timeFreqData[jj] = new float[cols];
            }
            timeFreqData = stft(compX, wSamp);
        }

        float[][] stft(Complex[] x, int wSamp) {
            
            Stopwatch test = new Stopwatch();

            int ii = 0;
            int jj = 0;
            int kk = 0;
            int ll = 0;
            int N = x.Length;

            float fftMax = 0;
            int loopCount = 2 * (int)Math.Floor((double)N / (double)wSamp) - 1;
            
            float[][] Y = new float[wSamp / 2][];
            for (ll = 0; ll < wSamp / 2; ll++) {
                Y[ll] = new float[2 * (int)Math.Floor((double)N / (double)wSamp)];

            }

      /*      void Worker(int p) {
                int i = (int)p;
                Complex[] temp2 = new Complex[wSamp];
                Complex[] tempFFT2 = new Complex[wSamp];
                for (int j = 0; j < wSamp; j++) {
                    temp2[j] = x[i * (wSamp / 2) + j];
                }

                tempFFT2 = fft(temp2);
                // wsamp = 2048
                for (int k = 0; k < wSamp / 2; k++) {
                    Y[k][i] = (float)Complex.Abs(tempFFT2[k]);

                    if (Y[k][i] > fftMax) {
                        fftMax = Y[k][i];
                    }
                }
            }*/

            void Worker2(int start, int end) {
                Complex[] temp2 = new Complex[wSamp];
                Complex[] tempFFT2 = new Complex[wSamp];

                for (int i = start; i < end; i++) {
                    for (int j = 0; j < wSamp; j++) {
                        temp2[j] = x[i * (wSamp / 2) + j];
                    }

                    tempFFT2 = fft(temp2);
                    
                    for (int k = 0; k < wSamp / 2; k++) {
                        Y[k][i] = (float)Complex.Abs(tempFFT2[k]);

                        if (Y[k][i] > fftMax) {
                            fftMax = Y[k][i];
                        }
                    }
                }
            }

            MainWindow.DoWorkInParallel(Worker2, loopCount);
            
        

            /*  var tasks = new List<Task>();
              for (int p = 0; p < asd; p++) {
                  int index = p;
                  tasks.Add(Task.Run(() => Worker(index)));
                  //  Worker();
              }*/
            
            //  Task.WaitAll(tasks.ToArray());

            /*  Parallel.For(0, loopCount ,i => {
                  Complex[] temp2 = new Complex[wSamp];
                  Complex[] tempFFT2 = new Complex[wSamp];
                  for (int j = 0; j < wSamp; j++) {
                        temp2[j] = x[i * (wSamp / 2) + j];
                    }

                    tempFFT2 = fft(temp2);
                  
                    for (int k = 0; k < wSamp / 2; k++) {
                        Y[k][i] = (float)Complex.Abs(tempFFT2[k]);

                        if (Y[k][i] > fftMax) {
                          fftMax = Y[k][i];
                      }
                    }
                });*/

            
            /*     Parallel.For(0, threadCount, p => {
                     int end = amountPartition * (p + 1) + (p != 0 ? leftOver : 0); //offset the ending for the rest of the thread
                     int start = end - amountPartition;
                     end += (p == 0 ? leftOver : 0); //add leftOver to the first thread 'after' assigning declaring so that 'start' will have value of 0
                     Complex[] temp2 = new Complex[wSamp];
                     Complex[] tempFFT2 = new Complex[wSamp];

                     for (int i = start; i < end; i++) {
                         for (int j = 0; j < wSamp; j++) {
                             temp2[j] = x[i * (wSamp / 2) + j];
                         }

                         tempFFT2 = fft(temp2);
                         // wsamp = 2048
                         for (int k = 0; k < wSamp / 2; k++) {
                             Y[k][i] = (float)Complex.Abs(tempFFT2[k]);

                             if (Y[k][i] > fftMax) {
                                 fftMax = Y[k][i];
                             }
                         }
                     }
                 });*/

         
           
               void Worker3(int start, int end) {
                   for (int i = start; i < end; i++) {
                       for (int k = 0; k < wSamp / 2; k++) {
                           Y[k][i] /= fftMax;
                       }
                   }
               }

         
            MainWindow.DoWorkInParallel(Worker3, loopCount);

            /*  Parallel.For(0, threadCount, p => {
                  int end = amountPartition * (p + 1) + (p != 0 ? leftOver : 0); //offset the ending for the rest of the thread
                  int start = end - amountPartition;
                  end += (p == 0 ? leftOver : 0); //add leftOver to the first thread 'after' assigning declaring so that 'start' will have value of 0
                  for (int i = start; i < end; i++) {
                      for (int k = 0; k < wSamp / 2; k++) {
                          Y[k][i] /= fftMax;
                      }
                  }
              });*/

            return Y;
        }

      

        Complex[] fft(Complex[] x) {
            int ii = 0;
            int kk = 0;
            int N = x.Length;

            Complex[] Y = new Complex[N];

            // NEED TO MEMSET TO ZERO?

            if (N == 1) {
                Y[0] = x[0];
            } else {

                Complex[] E = new Complex[N / 2];
                Complex[] O = new Complex[N / 2];
                Complex[] even = new Complex[N / 2];
                Complex[] odd = new Complex[N / 2];


               
                for (ii = 0; ii < N; ii++) {

                    if (ii % 2 == 0) {
                        even[ii / 2] = x[ii];
                    }
                    if (ii % 2 == 1) {
                        odd[(ii - 1) / 2] = x[ii];
                    }
                }
               


                E = fft(even);
                O = fft(odd);



                for (kk = 0; kk < N; kk++) {
                    Y[kk] = E[(kk % (N / 2))] + O[(kk % (N / 2))] * twiddles[kk * wSamp / N];
                }
            }

            return Y;
        }

    }
}
