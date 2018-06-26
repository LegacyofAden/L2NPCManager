using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace GeoEngine.Utils
{
    public class Util
    {
        public delegate int CompareFunc<T>(T val_1, T val_2);


        public interface FastComparator<T> {
            bool compare(T o1, T o2);
        }
        
        public static int arrayFirstIndexOf<T>(T[] array, T value) {
            int c = array.Length;
            for (int i = 0; i < c; i++) {
                if (array[i].Equals(value)) return i;
            }
            return -1;
        }
        
        public static int arrayLastIndexOf<T>(T[] array, T value) {
            for (int i = array.Length; i-- > 0;) {
                if (array[i].Equals(value)) return i;
            }
            return -1;
        }
        
        public static T[] arrayAdd<T>(T[] array, T value) {
            int size = array.Length;
            T[] new_array = new T[size];
            Array.Copy(array, new_array, size + 1);
            new_array[size] = value;
            return array;
        }
        
        public static T[] arrayRemoveFirst<T>(T[] array, T value) {
            int index = arrayFirstIndexOf(array, value);
            if (index != -1) return arrayRemoveAtUnsafe(array, index);
            return array;
        }
        
        public static T[] arrayRemoveLast<T>(T[] array, T value) {
            int index = arrayLastIndexOf(array, value);
            if (index != -1) return arrayRemoveAtUnsafe(array, index);
            return array;
        }
        
        public static T[] arrayRemoveAtUnsafe<T>(T[] array, int index) {
            T[] newArray = new T[array.Length - 1];
            if (index != 0) Array.Copy(array, 0, newArray, 0, index);
            Array.Copy(array, index + 1, newArray, index, newArray.Length - index);
            return newArray;
        }

        public static void quickSort<T>(T[] values, CompareFunc<T> comparator) {
            quickSort(values, values.Length, comparator);
        }

        public static void quickSort<T>(T[] values, int length, CompareFunc<T> comparator) {
            if (length > 1) {
                int high = length - 1;
                quickSortImpl(values, 0, high, comparator);
                insertionSort(values, 0, high, comparator);
            }
        }

        private static void quickSortImpl<T>(T[] values, int low, int high, CompareFunc<T> comparator) {
            int i;
            int j;
            T v;
            //
            if (high - low > 4) {
                i = (high + low) / 2;
                if (comparator.Invoke(values[low], values[i]) != 0) swap(values, low, i);
                if (comparator.Invoke(values[low], values[high]) != 0) swap(values, low, high);
                if (comparator.Invoke(values[i], values[high]) != 0) swap(values, i, high);
                //
                j = high - 1;
                swap(values, i, j);
                i = low;
                v = values[j];
                while (true) {
                    while (comparator.Invoke(v, values[++i]) != 0) {}
                    while (comparator.Invoke(values[--j], v) != 0) {}
                    if (j < i) break;
                    swap(values, i, j);
                }
                swap(values, i, high - 1);
                quickSortImpl(values, low, j, comparator);
                quickSortImpl(values, i + 1, high, comparator);
            }
        }
        
        private static void insertionSort<T>(T[] values, int low, int high, CompareFunc<T> comparator) {
            T v;
            for (int i = low + 1, j; i <= high; i++) {
                v = values[i];
                j = i;
                //
                while ((j > low) && comparator.Invoke(values[j - 1], v) > 0) {
                    values[j] = values[j - 1];
                    j--;
                }
                values[j] = v;
            }
        }
        
        public static void mergeSort<T>(T[] src, T[] dest, int length, IComparer<T> comparator) {
            mergeSort(src, dest, 0, length, comparator);
        }
        
        private static void mergeSort<T>(T[] src, T[] dest, int low, int high, IComparer<T> comparator) {
            int length = high - low;
            //
            // Insertion sort on smallest arrays
            if (length < 7) {
                for (int i = low; i < high; i++) {
                    for (int j = i; j > low && comparator.Compare(dest[j - 1], dest[j]) > 0; j--) {
                        swap(dest, j, j - 1);
                    }
                }
                return;
            }
            //
            // Recursively sort halves of dest into src
            int destLow = low;
            int destHigh = high;
            //int mid = (low + high) >>> 1;
            int mid =  (int)((uint)(low + high) >> 1);
            mergeSort(dest, src, low, mid, comparator);
            mergeSort(dest, src, mid, high, comparator);
            //
            // If list is already sorted, just copy from src to dest.  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (comparator.Compare(src[mid - 1], src[mid]) <= 0) {
                Array.Copy(src, low, dest, destLow, length);
                return;
            }
            //
            // Merge sorted halves (now in src) into dest
            for (int i = destLow, p = low, q = mid; i < destHigh; i++) {
                if (q >= high || p < mid && comparator.Compare(src[p], src[q]) <= 0) {
                    dest[i] = src[p++];
                } else {
                    dest[i] = src[q++];
                }
            }
        }
        
        public static void mergeSort<T>(T[] src, T[] dest, int length, FastComparator<T> comparator) {
            mergeSort(src, dest, 0, length, comparator);
        }
        
        private static void mergeSort<T>(T[] src, T[] dest, int low, int high, FastComparator<T> comparator) {
            int length = high - low;
            //
            // Insertion sort on smallest arrays
            if (length < 7) {
                for (int i = low; i < high; i++) {
                    for (int j = i; j > low && comparator.compare(dest[j - 1], dest[j]); j--) {
                        swap(dest, j, j - 1);
                    }
                }
                return;
            }
            //
            // Recursively sort halves of dest into src
            int destLow = low;
            int destHigh = high;
            //int mid = (low + high) >>> 1;
            int mid =  (int)((uint)(low + high) >> 1);
            mergeSort(dest, src, low, mid, comparator);
            mergeSort(dest, src, mid, high, comparator);
            //
            // If list is already sorted, just copy from src to dest.  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (!comparator.compare(src[mid - 1], src[mid])) {
                Array.Copy(src, low, dest, destLow, length);
                return;
            }
            //
            // Merge sorted halves (now in src) into dest
            for (int i = destLow, p = low, q = mid; i < destHigh; i++) {
                if (q >= high || p < mid && !comparator.compare(src[p], src[q])) {
                    dest[i] = src[p++];
                } else {
                    dest[i] = src[q++];
                }
            }
        }
        
        private static void swap<T>(T[] x, int a, int b) {
            T t = x[a];
            x[a] = x[b];
            x[b] = t;
        }

        public static BitmapImage loadImage(string filename) {
            if (!File.Exists(filename)) return null;
            try {return new BitmapImage(new Uri(filename));}
            catch (IOException e) {
                Console.WriteLine(e);
                return null;
            }
        }

        public static void saveImage(string filename, BitmapImage img, string format) {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(img));
            using (FileStream stream = File.Open(filename, FileMode.Create, FileAccess.Write)) {
                encoder.Save(stream);
            }
        }
        
        /**
         * Scales the source image to the given width/height.<br>
         * Faster then BufferedImage.getScaledInstance(width, height, hints) and returning an BufferedImage instead of an ToolkitImage.
         * 
         * @param img The image to be scaled
         * @param width The resulting image width
         * @param height The resulting image height
         * @param quality The quality level (0 = lowest, 1 = medium, 2 = highest, anything else = default)
         * @return The scaled image
         */
        //public static BufferedImage scaleImage(BufferedImage img, int width, int height, int quality) {
        //    if (img.getWidth() == width && img.getHeight() == height) return img;
        //    //
        //    BufferedImage scaled = new BufferedImage(width, height, img.getType() == 0 ? BufferedImage.TYPE_4BYTE_ABGR : img.getType());
        //    Graphics2D g = scaled.createGraphics();
        //    //
        //    switch (quality) {
        //        case 0:
        //            g.setRenderingHint(RenderingHints.KEY_RENDERING, RenderingHints.VALUE_RENDER_SPEED);
        //            g.setRenderingHint(RenderingHints.KEY_INTERPOLATION, RenderingHints.VALUE_INTERPOLATION_NEAREST_NEIGHBOR);
        //            break;
        //        case 1:
        //            g.setRenderingHint(RenderingHints.KEY_RENDERING, RenderingHints.VALUE_RENDER_SPEED);
        //            g.setRenderingHint(RenderingHints.KEY_INTERPOLATION, RenderingHints.VALUE_INTERPOLATION_BILINEAR);
        //            break;
                                
        //        case 2:
        //            g.setRenderingHint(RenderingHints.KEY_RENDERING, RenderingHints.VALUE_RENDER_QUALITY);
        //            g.setRenderingHint(RenderingHints.KEY_INTERPOLATION, RenderingHints.VALUE_INTERPOLATION_BICUBIC);
        //            break;
        //    }
        //    //
        //    g.drawImage(img, 0, 0, width, height, null);
        //    g.dispose();
        //    //
        //    return scaled;
        //}
        
        public static void writeByte(int value, Stream os) {
            os.WriteByte((byte)(value & 0xFF));
        }
        
        public static void writeBytes(byte[] values, Stream os) {
            os.Write(values, 0, values.Length);
        }
        
        public static void writeInt(int value, Stream os) {
            os.WriteByte((byte)(value & 0xFF));
            os.WriteByte((byte)(value >> 8 & 0xFF));
            os.WriteByte((byte)(value >> 16 & 0xFF));
            os.WriteByte((byte)(value >> 24 & 0xFF));
        }
        
        public static void writeShort(int value, Stream os) {
            os.WriteByte((byte)(value & 0xFF));
            os.WriteByte((byte)(value >> 8 & 0xFF));
        }
                
        public static string capitalizeString(string stream) {
            if (string.IsNullOrEmpty(stream)) return stream;
            //
            int c = stream.Length;
            StringBuilder sb = new StringBuilder(c);
            sb.Append(char.ToUpper(stream[0]));
            for (int i = 1; i < c; i++) {
                sb.Append(char.ToLower(stream[i]));
            }
            return sb.ToString();
        }
    }
}