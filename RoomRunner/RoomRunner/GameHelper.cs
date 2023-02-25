using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Collections;

namespace GameHelper
{
    /**
     * <summary>
     * This class has several methods that can help do things faster. To use, put this method in the 'Initialize' method of Game1: <see cref="Setup(Game1)"/><br/>
     * What that will do is set the defaults of <see cref="SpriteBatch"/> and <see cref="ContentManager"/>.<br/>
     * If you want a texture, there is a method is the class 'SheetReader' that looks like this: <see cref="TextureLoader.GetName{T}(string)"/><br/>
     * This can be used to get any file by it's name. (do not include the .png or any other extension as part of the name!)
     * </summary>
     * 
     */
    public static class GameHelper
    {
        //SETTINGS (public const / readonly static)

        public const bool AutoLoad = true; //If true, attempts to load default save name for the TextureLoader class.
        public const bool AutoSave = true; //If true, attempts to auto save if the load fails and it has to find all files the hard way.

        public const string TextureLoaderSaveName = "Filetypes"; //Default name used for the TextureLoader class save file.
        public const string SheetReaderSaveName = "Sprites"; //Default name used for the SheetReader class save file.
        public const string HitboxesSaveName = "Hitboxes"; //Default name used for the Hitboxes class save file.
        public const string DefaultSFName = "SpriteFont1"; //Default name of the spritefont file used for the default.
        public const string DefaultSpriteSheetName = "sheet"; //Default name of the sprite sheet file.
        public const string DefaultGetDefaultErrMessage = "No type provided while default type is null. Either set a default or give a type."; //Default error message thrown when getting a default. No very important but I like it :)

        public readonly static Type[] DefaultTypes = new Type[] { typeof(SpriteFont), typeof(Texture2D) }; //Default types used for loading in TextureLoader.
        public readonly static Type[] TypesInFile = new Type[] { typeof(GameHelper), typeof(Sorter), typeof(SheetReader), typeof(Hitboxes), typeof(TextureLoader) }; //All classes in this cs file (that aren't inside other classes).
        public readonly static Color[] DefaultBannedColors = new Color[] { Color.White }; //Default colors banned in SheetReader.
        public readonly static Type[] TypesWithDefaults = new Type[] { typeof(ContentManager), typeof(SpriteBatch), typeof(GraphicsDeviceManager), typeof(SpriteFont) }; //Types that have a default variable down below here.
        public readonly static string[] DefaultPointers = new string[] { "DefaultCM_FI", "DefaultSB_FI", "DefaultGDM_FI", "" }; //This array works with the one above to identify the default var. This is done to make it easier to add more default vars later. 
        public readonly static string[] DefaultGetters = new string[] { "DefaultCM", "DefaultSB", "DefaultGDM", "DefaultSF" }; //This array works the TypeWithDefaults array to identify the getter of a default var.

        //PUBLIC STATIC VARIABLES

        public static Game1 Game; //The Game1 object, the main class used to make games. Needed so that I can use reflection with it to steal vars.
        public static FieldInfo DefaultCM_FI; //Pointer to the ContentManager from the Game1 class. Use the DefaultCM to get the value.
        public static FieldInfo DefaultSB_FI; //Pointer to the SpriteBatch from the Game1 class. Use the DefaultSB to get the value.
        public static FieldInfo DefaultGDM_FI; //Pointer to the GraphicsDeviceManager from Game1 class. Use the DefaultGDM to get the value.
        public static KeyboardState oldKb; //Is the last keyboard state used (changed each update).

        //PRIVATE STATIC VARIABLES

        private static Texture2D Pixel; //1x1 pixel texture to use a default if needed.

        //PUBLIC STATIC METHOD VARIABLES
        public static ContentManager DefaultCM => (ContentManager)DefaultCM_FI.GetValue(Game); //Default ContentManager from the Game1 class. This should be used 99% of the time.
        public static SpriteBatch DefaultSB => (SpriteBatch)DefaultSB_FI.GetValue(Game); //Default SpriteBatch from the Game1 class. This should be used 99% of the time.
        public static GraphicsDeviceManager DefaultGDM => (GraphicsDeviceManager)DefaultGDM_FI.GetValue(Game); //Default GraphicsDeviceManager from the Game1 class. This should be used 99% of the time.
        public static SpriteFont DefaultSF => TextureLoader.GetName<SpriteFont>(DefaultSFName); //Default SpriteFont from the Game1 class. This should be used 99% of the time.
        public static Rectangle Frame => DefaultGDM.GraphicsDevice.Viewport.Bounds; //Frame from the Game1 class. Used to prevent compilation errors so that this class can yell at the person if Frame isn't found.
        public static Texture2D DefaultSpritesheet => TextureLoader.GetName<Texture2D>(DefaultSpriteSheetName); //Default spritesheet, if there is one. From the TextureLoader class.
        public static Texture2D DefaultPixel => Pixel; //Default Pixel (a 1x1 white texture2D), if there is one.

        //CONSTRUCTORS

        static GameHelper()
        {
            oldKb = Keyboard.GetState();
        }

        //PUBLIC STATIC METHODS
        public static void Setup(Game1 game)
        {
            Game = game;
            try
            {
                List<FieldInfo> vars = typeof(Game1).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList();
                vars.AddRange(typeof(Game).GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
                vars.RemoveAll(a => !TypesWithDefaults.Contains(a.FieldType));
                List<FieldInfo> varsToSet = GetDefaults();
                foreach (FieldInfo fi in varsToSet)
                {
                    Type T = TypesWithDefaults[Array.IndexOf(DefaultPointers,fi.Name)];
                    fi.SetValue(null, vars.First(a => a.FieldType == T));
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + "Was not able to find a SpriteBatch and/or a ContentLoader variable in Game1.");
            }

            Pixel = new Texture2D(DefaultGDM.GraphicsDevice, 1, 1);
            Pixel.SetData(new Color[] { Color.White });

            if (AutoLoad && !TextureLoader.HasLoaded)  
            if (!TextureLoader.Load() || !TextureLoader.HasLoaded)
            {
                TextureLoader.FindFilesOfTypes();
                if (AutoSave) TextureLoader.Save();
            }
        }
        private static List<FieldInfo> GetDefaults() => typeof(GameHelper).GetFields(BindingFlags.Public | BindingFlags.Static).ToList().FindAll(a => DefaultPointers.Contains(a.Name));
        public static T GetDefault<T>(T item = default) where T : class
        {
            T thing = typeof(GameHelper).GetProperties(BindingFlags.Public | BindingFlags.Static).ToList().First(a => a.PropertyType == typeof(T) && DefaultGetters.Contains(a.Name)).GetValue(null, null) as T;
            if (thing == null && Game == null) throw new Exception(DefaultGetDefaultErrMessage.Replace("type", typeof(T).Name));
            return thing;
        }
        public static T[] GetDefault<T>(params T[] items) where T : class
        {
            if (items.Length == 0) return new T[] { GetDefault<T>() };
            T[] outp = new T[items.Length];
            for (int i = 0; i < outp.Length; i++) outp[i] = GetDefault(items[i]);
            return outp;
        }
        public static string[] RectanglesToString(Rectangle[] r)
        {
            List<string> outp = new List<string>();

            foreach (Rectangle rr in r)
                outp.Add(rr.X + " " + rr.Y + " " + rr.Width + " " + rr.Height);

            return outp.ToArray();
        }
        public static List<string> RectanglesToStrList(Rectangle[] r)
        {
            List<string> outp = new List<string>();

            foreach (Rectangle rr in r)
                outp.Add(rr.X + " " + rr.Y + " " + rr.Width + " " + rr.Height);

            return outp;
        }
        public static string RectangleToString(Rectangle r)
        {
            return r.X + " " + r.Y + " " + r.Width + " " + r.Height;
        }
        public static string[] ReadFile(string fileName)
        {
            if (!File.Exists(@fileName)) throw new FileNotFoundException("Path was invalid.");
            string[] spl = fileName.Split('/');
            string hold = spl[spl.Length - 1].Split('.')[0];
            if (TextureLoader.SavedContent != null && TextureLoader.SavedContent.Keys.Contains(hold)) 
                return TextureLoader.SavedContent[hold]; 
            List<string> outp = new List<string>();
            using (StreamReader sr = new StreamReader(@fileName))
                while (!sr.EndOfStream)
                    outp.Add(sr.ReadLine());
            return outp.ToArray();
        }        
        public static void Draw(string s, Vector2 pos, Color col = default, SpriteBatch sb = null, SpriteFont sf = null)
        {
            if (col == default) col = Color.Black;
            if (sb == null) sb = GetDefault(sb);
            if (sf == null) sf = GetDefault(sf);
            sb.DrawString(sf, s, pos, col);
        }
        public static bool IsPressed(Keys k, KeyboardState kb) => kb.IsKeyDown(k) && !oldKb.IsKeyDown(k);
        public static Type GetCallingClass(int skipFrames = 2) => new StackFrame(skipFrames, false).GetMethod().DeclaringType;
        public static Dictionary<FieldInfo, Type> FindFieldsOfInterfaces(Type T, params Type[] interfaces)
        {
            if (interfaces.Length == 0) return default;
            Dictionary<FieldInfo, Type> outp = new Dictionary<FieldInfo, Type>();
            FieldInfo[] vars = T.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).ToArray();
            foreach (FieldInfo fi in vars)
            {
                Type[] fiInts = fi.FieldType.GetInterfaces();
                foreach (Type i in interfaces) 
                    if (fiInts.Any(a => a == i))
                    {
                        outp[fi] = i;
                        break;
                    }
            }
            return outp;
        }
        public static T[] RemoveNulls<T>(IEnumerable<T> arr) where T : class
        {
            List<T> outp = new List<T>();
            foreach (T item in arr) if (!(item is null)) outp.Add(item);
            return outp.ToArray();
        }
        public static T[,] RemoveNulls<T>(T[,] arr) where T : class
        {
            List<int> rowsToRemove = new List<int>(), columnsToRemove = new List<int>();
            for (int r = 0; r < arr.GetLength(0); r++)
            {
                bool allNulls = true;
                for (int c = 0; c < arr.GetLength(1); c++)
                    if (arr[r, c] != null) allNulls = false;
                if (allNulls) rowsToRemove.Add(r);
            }
            for (int c = 0; c < arr.GetLength(1); c++)
            {
                bool allNulls = true;
                for (int r = 0; r < arr.GetLength(0); r++)
                    if (arr[r, c] != null) allNulls = false;
                if (allNulls) columnsToRemove.Add(c);
            }
            T[,] outp = new T[arr.GetLength(0) - rowsToRemove.Count, arr.GetLength(1) - columnsToRemove.Count];
            for (int r = 0, outR = 0; r < arr.GetLength(0); r++)
            {
                if (rowsToRemove.Contains(r)) continue;
                for (int c = 0, outC = 0; c < arr.GetLength(1); c++)
                {
                    if (columnsToRemove.Contains(c)) continue;
                    outp[outR, outC] = arr[r, c];
                    outC++;
                }
                outR++;
            }
            return outp;
        }
        public static T[] ReplaceWith<T>(T[] arr, T toReplace, T replaceWith)
        {
            for (int i = 0; i < arr.Length; i++) if (arr[i].Equals(toReplace)) arr[i] = replaceWith;
            return arr;
        }
        public static T[,] ReplaceWith<T>(T[,] arr, T toReplace, T replaceWith)
        {
            for (int r = 0; r < arr.GetLength(0); r++)
                for (int c = 0; c < arr.GetLength(1); c++)
                    if ((arr[r, c] != null && arr[r, c].Equals(toReplace)) || (arr[r, c] == null && toReplace == null)) 
                        arr[r, c] = replaceWith;
            return arr;
        }
        public static T[] Combine<T>(params T[][] arrs)
        {
            int length = 0;
            foreach (T[] arr in arrs) length += arr.Length;
            T[] outp = new T[length];
            length = 0;
            foreach (T[] arr in arrs) 
                for (int i = 0; i < arr.Length; i++, length++) 
                    outp[length] = arr[i];
            return outp;
        }
        public static T[] RemoveDupes<T>(T[] arr)
        {
            List<T> used = new List<T>();
            foreach (T thing in arr) if (used.Contains(thing)) continue; else used.Add(thing);
            return used.ToArray();
        }
        public static T[][] ConvertArr<T>(T[,] arr)
        {
            T[][] outp = new T[arr.GetLength(0)][];
            for (int r = 0; r < arr.GetLength(0); r++)
            {
                outp[r] = new T[arr.GetLength(1)];
                for (int c = 0; c < arr.GetLength(1); c++)
                    outp[r][c] = arr[r, c];
            }
            return outp;
        }
        public static T[,] ConvertArr<T>(T[][] arr)
        {
            T[,] outp = new T[arr.Length, arr[0].Length];
            for (int r = 0; r < arr.Length; r++)
                for (int c = 0; c < arr[0].Length; c++)
                    outp[r, c] = arr[r][c];
            return outp;
        }
        public static T[,] FlipArr<T>(T[,] arr)
        {
            T[,] outp = new T[arr.GetLength(1), arr.GetLength(0)];
            for (int r = 0; r < arr.GetLength(0); r++)
                for (int c = 0; c < arr.GetLength(1); c++)
                    outp[c, r] = arr[r, c];
            return outp;
        }
        public static T[][] FlipArr<T>(T[][] arr)
        {
            T[][] outp = new T[arr[0].Length][];
            for (int i=0;i<outp.Length;i++) 
                outp[i] = new T[arr.Length];
            for (int r = 0; r < arr.Length; r++)
                for (int c = 0; c < arr[0].Length; c++)
                    outp[c][r] = arr[r][c];
            return outp;
        }
        public static HashSet<T> UniqueElements<T>(T[,] arr)
        {
            HashSet<T> outp = new HashSet<T>();
            foreach (T item in arr) outp.Add(item);
            return outp;
        }
        public static S GetAll<T, S>(T[][] arr, S str, Action<T, S> todo)
        {
            foreach (T[] arrItem in arr) foreach (T item in arrItem) todo.Invoke(item, str);
            return str;
        }
        public static IEnumerable SetNamesOfTextures(IEnumerable arr)
        {
            foreach (Texture2D item in arr)
                if (item == null || item.Name != "") continue;
                else item.Name = TextureLoader.FindName(item);
            return arr;
        }
        public static T[] TwoDimToOneDim<T>(T[][] arr)
        {
            T[] outp = new T[arr.Length * arr[0].Length];
            for (int r = 0, i = 0; r < arr.Length; r++)
                for (int c = 0; c < arr[r].Length; c++, i++)
                    outp[i] = arr[r][c];
            return outp;
        }
    }
    public static class Updater
    {
        private readonly static Dictionary<Type, Dictionary<FieldInfo, Type>> LoadedUpdates;
        private readonly static HashSet<object> InstancesToUpdate;

        public static bool Paused;

        static Updater()
        {
            LoadedUpdates = new Dictionary<Type, Dictionary<FieldInfo, Type>>();
            InstancesToUpdate = new HashSet<object>();
            Paused = false;
        }
        public static void UpdateAll<T>(IEnumerable<T> arr) where T : IUpdatable
        {
            foreach (T thing in arr) thing.Update();
        }
        public static void UpdateAll<T>(IEnumerable<T> arr, GameTime gt) where T : IUpdatableGT
        {
            foreach (T thing in arr) thing.Update(gt);
        }
        public static void UpdateAll<T>(params T[] arr) where T : IUpdatable
        {
            foreach (T thing in arr) thing.Update();
        }
        public static void UpdateAll<T>(GameTime gt, params T[] arr) where T : IUpdatableGT
        {
            foreach (T thing in arr) thing.Update(gt);
        }
        public static void UpdateAll<T>(T instance, GameTime gameTime = default)
        {
            Type TT = typeof(T);
            if (LoadedUpdates.ContainsKey(TT))
            {
                UpdateAll(LoadedUpdates[TT], instance, gameTime);
                GameHelper.oldKb = Keyboard.GetState();
                return;
            }
            bool gt = gameTime != default;
            Dictionary<FieldInfo, Type> outp = new Dictionary<FieldInfo, Type>();
            FieldInfo[] vars = TT.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).ToArray();
            foreach (FieldInfo fi in vars)
            {
                Type[] interfaces = fi.FieldType.GetInterfaces();
                if (interfaces.Length == 0) continue;
                object val = fi.GetValue(instance);
                if (interfaces.Any(a => a == typeof(IUpdatable)))
                {
                    outp.Add(fi, typeof(IUpdatable));
                    if (val is null) continue;
                    (val as IUpdatable).Update();
                    continue;
                }
                if (!gt) continue;
                if (interfaces.Any(a => a == typeof(IUpdatableGT)))
                {
                    outp.Add(fi, typeof(IUpdatableGT));
                    if (val is null) continue;
                    (val as IUpdatableGT).Update(gameTime);
                }
            }
            LoadedUpdates[TT] = outp;
            GameHelper.oldKb = Keyboard.GetState();
        }
        public static void UpdateAll(GameTime gameTime = default)
        {
            foreach (object o in InstancesToUpdate) UpdateAll(LoadedUpdates[o.GetType()], o, gameTime);
            GameHelper.oldKb = Keyboard.GetState();
        }
        
        private static void UpdateAll<T>(Dictionary<FieldInfo, Type> key, T instance, GameTime gameTime = default)
        {
            if (Paused) return;
            IOrderedEnumerable<KeyValuePair<FieldInfo, Type>> arr = key.OrderByDescending(a => a.Key.GetValue(instance) as IPriorityUpdate, new Priority());
            foreach (KeyValuePair<FieldInfo, Type> kvp in arr)
            {
                object val = kvp.Key.GetValue(instance);
                if (val is null) continue;
                if (key[kvp.Key] == typeof(IUpdatable)) (val as IUpdatable).Update();
                else (val as IUpdatableGT).Update(gameTime);
            }
        }
        private static Dictionary<FieldInfo, Type> FindUpdatableFields(Type t) => GameHelper.FindFieldsOfInterfaces(t, typeof(IUpdatable), typeof(IUpdatableGT));
        public static void AddUpdatableInstances(params object[] toAdd)
        {
            foreach (object o in toAdd)
            {
                if (!LoadedUpdates.ContainsKey(o.GetType())) LoadedUpdates[o.GetType()] = FindUpdatableFields(o.GetType());
                InstancesToUpdate.Add(o);
            }
        }
    }
    public static class Drawer
    {
        private readonly static Dictionary<Type, Dictionary<FieldInfo, Type>> LoadedDrawables;
        private readonly static HashSet<object> InstancesToUpdate;

        public static bool Paused;

        static Drawer()
        {
            LoadedDrawables = new Dictionary<Type, Dictionary<FieldInfo, Type>>();
            InstancesToUpdate = new HashSet<object>();
            Paused = false;
        }

        public static void DrawAll<T>(IEnumerable<T> arr) where T : IDrawable
        {
            arr = Sorter.MergeSort(arr.ToArray(), new Priority() as IComparer<T>);
            foreach (T thing in arr) thing.Draw();
        }
        public static void DrawAll<T>(IEnumerable<T> arr, Vector2 pos) where T : IDrawablePos
        {
            arr = Sorter.MergeSort(arr.ToArray(), new Priority() as IComparer<T>);
            foreach (T thing in arr) thing.Draw(pos);
        }
        public static void DrawAll<T>(params T[] arr) where T : IDrawable
        {
            arr = Sorter.MergeSort(arr.ToArray(), new Priority() as IComparer<T>);
            foreach (T thing in arr) thing.Draw();
        }
        public static void DrawAll<T>(Vector2 pos, params T[] arr) where T : IDrawablePos
        {
            arr = Sorter.MergeSort(arr.ToArray(), new Priority() as IComparer<T>);
            foreach (T thing in arr) thing.Draw(pos);
        }
        public static void DrawAll<T>(T instance, Vector2 pos = default)
        {
            Type TT = typeof(T);
            if (LoadedDrawables.ContainsKey(TT))
            {
                DrawAll(LoadedDrawables[TT], instance, pos);
                return;
            }
            Dictionary<FieldInfo, Type> outp = FindDrawableFields(TT);
            DrawAll(outp, instance, pos);
            LoadedDrawables[TT] = outp;
        }
        public static void DrawAll(Vector2 pos = default)
        {
            foreach (object o in InstancesToUpdate) DrawAll(LoadedDrawables[o.GetType()], o, pos);
        }

        private static void DrawAll<T>(Dictionary<FieldInfo, Type> key, T instance, Vector2 pos = default)
        {
            if (Paused) return;
            IOrderedEnumerable<KeyValuePair<FieldInfo, Type>> arr = key.OrderBy(a => a.Key.GetValue(instance) as IPriorityDraw, new Priority());
            foreach (KeyValuePair<FieldInfo, Type> kvp in arr)
            {
                object val = kvp.Key.GetValue(instance);
                if (val is null) continue;
                if (key[kvp.Key] == typeof(IDrawable)) (val as IDrawable).Draw();
                else (val as IDrawablePos).Draw(pos);
            }
        }
        private static Dictionary<FieldInfo, Type> FindDrawableFields(Type t) => GameHelper.FindFieldsOfInterfaces(t, typeof(IDrawable), typeof(IDrawablePos));
        public static void AddUpdatableInstances(params object[] toAdd)
        {
            foreach (object o in toAdd)
            {
                if (!LoadedDrawables.ContainsKey(o.GetType())) LoadedDrawables[o.GetType()] = FindDrawableFields(o.GetType());
                InstancesToUpdate.Add(o);
            }
        }
    }
    public class Priority : IComparer, IComparer<IPriorityUpdate>, IComparer<IPriorityDraw>
    {
        public int Compare(object o1, object o2)
        {
            if (o1 is null)
                if (o2 is null)
                    return 0;
                else
                    return -1;
            else
                if (o2 is null)
                    return 1;

            if (o1 is IPriorityUpdate && o2 is IPriorityUpdate) return Compare(o1 as IPriorityUpdate, o2 as IPriorityUpdate);
            if (o1 is IPriorityDraw && o2 is IPriorityDraw) return Compare(o1 as IPriorityDraw, o2 as IPriorityDraw);

            throw new ArgumentException("Objects do not have any priorities.");
        }
        public int Compare(IPriorityUpdate p1, IPriorityUpdate p2) => p1 is null ? p2 is null ? 0 : -1 : p2 is null ? 1 : p1.GetUpdatePriority() - p2.GetUpdatePriority();
        public int Compare(IPriorityDraw p1, IPriorityDraw p2) => p1 is null ? p2 is null ? 0 : -1 : p2 is null ? 1 : p1.GetDrawPriority() - p2.GetDrawPriority();
    }
    public interface IPriorityUpdate
    {
        int GetUpdatePriority();
    }
    public interface IPriorityDraw
    {
        int GetDrawPriority();
    }
    public interface IUpdatableGT : IPriorityUpdate
    {
        void Update(GameTime gt);
    }
    public interface IUpdatable : IPriorityUpdate
    {
        void Update();
    }
    public interface IDrawablePos : IPriorityDraw
    {
        void Draw(Vector2 pos);
    }
    public interface IDrawable : IPriorityDraw
    {
        void Draw();
    }
    public static class Sorter
    {
        private const int DefaultRunsize = 32; //Default runsize for timsort.
        public static KeyValuePair<T, V>[] MergeSort<T, V>(KeyValuePair<T, V>[] arr) where V : IComparable<V>
        {
            if (arr.Length == 1) return arr;
            if (arr.Length == 2)
                if (arr[1].Value.CompareTo(arr[0].Value) <= 0) return Swap(arr);
                else return arr;
            KeyValuePair<T, V>[] a1 = arr.Take(arr.Length / 2).ToArray(), a2 = arr.Skip(arr.Length / 2).ToArray();
            a1 = MergeSort(a1);
            a2 = MergeSort(a2);
            for (int i = 0, b1 = 0, b2 = 0; i < arr.Length; i++)
                if (b1 >= a1.Length || (b2 < a2.Length && a2[b2].Value.CompareTo(a1[b1].Value) <= 0)) arr[i] = a2[b2++];
                else arr[i] = a1[b1++];
            return arr;
        }
        public static T[] MergeSort<T>(T[] arr) where T : IComparable<T> => KVPConverter(MergeSort(KVPConverter(arr)));
        public static T[] MergeSort<T>(T[] toSort, out int[] indexSwaps) where T : IComparable<T> => IndexSort(toSort, out indexSwaps, a => MergeSort(a));
        public static KeyValuePair<T, V>[] MergeSort<T, V>(KeyValuePair<T, V>[] arr, IComparer<V> comp)
        {
            if (arr.Length == 1) return arr;
            if (arr.Length == 2)
                if (comp.Compare(arr[1].Value,arr[0].Value) <= 0) return Swap(arr);
                else return arr;
            KeyValuePair<T, V>[] a1 = arr.Take(arr.Length / 2).ToArray(), a2 = arr.Skip(arr.Length / 2).ToArray();
            a1 = MergeSort(a1, comp);
            a2 = MergeSort(a2, comp);
            for (int i = 0, b1 = 0, b2 = 0; i < arr.Length; i++)
                if (b1 >= a1.Length || (b2 < a2.Length && comp.Compare(a2[b2].Value,a1[b1].Value) <= 0)) arr[i] = a2[b2++];
                else arr[i] = a1[b1++];
            return arr;
        }
        public static T[] MergeSort<T>(T[] arr, IComparer<T> comp) => KVPConverter(MergeSort(KVPConverter(arr), comp));
        public static T[] MergeSort<T>(T[] toSort, out int[] indexSwaps, IComparer<int> comp) => IndexSort(toSort, out indexSwaps, a => MergeSort(a, comp));
        public static KeyValuePair<T, V>[] SortedMergeSort<T, V>(KeyValuePair<T, V>[] arr1, KeyValuePair<T, V>[] arr2) where V : IComparable<V>
        {
            KeyValuePair<T, V>[] outp = new KeyValuePair<T, V>[arr1.Length + arr2.Length];
            for (int i1 = 0, i2 = 0, i3 = 0; i1 < outp.Length; i1++)
                if (i3 >= arr2.Length || (i2 < arr1.Length && arr1[i2].Value.CompareTo(arr2[i3].Value) <= 0)) outp[i1] = arr1[i2++];
                else outp[i1] = arr2[i3++];
            return outp;
        }
        public static T[] SortedMergeSort<T>(T[] arr1, T[] arr2) where T : IComparable<T> => KVPConverter(SortedMergeSort(KVPConverter(arr1), KVPConverter(arr2)));
        public static KeyValuePair<T,V>[] InsertionSort<T,V>(KeyValuePair<T,V>[] toSort, int startingIndex = 0, int endingIndex = -1) where V : IComparable<V>
        {
            if (endingIndex == -1) endingIndex = toSort.Length - 1;
            for (int i = startingIndex + 1; i <= endingIndex; i++) for (int j = i - 1; j >= startingIndex; j--)
                {
                    if (toSort[j].Value.CompareTo(toSort[j + 1].Value) <= 0) break;
                    Swap(toSort, j, j + 1);
                }
            return toSort;
        }
        public static T[] InsertionSort<T>(T[] arr, int start = 0, int end = -1) where T : IComparable<T> => KVPConverter(InsertionSort(KVPConverter(arr), start, end));
        public static T[] InsertionSort<T>(T[] arr, out int[] indexSwaps, int start = 0, int end = -1) where T : IComparable<T> => IndexSort(arr, out indexSwaps, a => InsertionSort(a, start, end));
        public static KeyValuePair<T, V>[] TimSort<T, V>(KeyValuePair<T, V>[] toSort, int runsize = DefaultRunsize) where V : IComparable<V> //https://www.chrislaux.com/timsort
        {
            int n = toSort.Length;
            for (int i = 0; i < n; i += runsize)
                InsertionSort(toSort, i, Math.Min(i + runsize - 1, n - 1));
            for (int size = runsize; size < n; size *= 2)
                for (int i = 0; i < n; i += 2 * size)
                    PutPart(toSort, SortedMergeSort(GetPart(toSort, i, i + size - 1), GetPart(toSort, i + size, Math.Min(i + 2 * size - 1, n - 1))), i);
            return toSort;
        }
        public static T[] TimSort<T>(T[] arr, int runsize = DefaultRunsize) where T : IComparable<T> => KVPConverter(TimSort(KVPConverter(arr), runsize));
        public static T[] TimSort<T>(T[] arr, out int[] indexSwaps, int runsize = DefaultRunsize) where T : IComparable<T> => IndexSort(arr, out indexSwaps, a => TimSort(a, runsize));
        public static int[] CountSort(int[] toSort, int min, int max)
        {
            int[] count = new int[max - min + 1], outp = new int[toSort.Length];
            foreach (int i in toSort) count[i]++;
            for (int i = 1; i < count.Length; i++) count[i] += count[i - 1];
            for (int i = count.Length - 1; i > 0; i--) count[i] = count[i - 1];
            for (int i = 0; i < toSort.Length; count[toSort[i]]++, i++) outp[count[toSort[i]]] = toSort[i];
            return outp;
        }
        public static T[] Swap<T>(T[] arr)
        {
            T item = arr[0];
            arr[0] = arr[1];
            arr[1] = item;
            return arr;
        }
        public static T[] Swap<T>(T[] arr, int ele1, int ele2)
        {
            T item = arr[ele1];
            arr[ele1] = arr[ele2];
            arr[ele2] = item;
            return arr;
        }
        public static KeyValuePair<T, V>[] SortPart<T, V>(KeyValuePair<T, V>[] arr, int startIndex, int endIndex, Func<KeyValuePair<T, V>[], KeyValuePair<T, V>[]> Sort = default) where V : IComparable<V>
        {
            if (Sort == default) Sort = a => MergeSort(a);
            KeyValuePair<T, V>[] toSort = new KeyValuePair<T, V>[endIndex - startIndex + 1];
            for (int i = startIndex, k = 0; i <= endIndex; i++, k++) toSort[k] = arr[i];
            toSort = Sort.Invoke(toSort);
            for (int i = startIndex, k = 0; i <= endIndex; i++, k++) arr[i] = toSort[k];
            return arr;
        }
        public static T[] SortPart<T>(T[] arr, int startIndex, int endIndex, Func<T[], T[]> Sort = default) where T : IComparable<T>
        {
            if (Sort == default) Sort = a => MergeSort(a);
            T[] toSort = new T[endIndex - startIndex + 1];
            for (int i = startIndex, k = 0; i <= endIndex; i++, k++) toSort[k] = arr[i];
            toSort = Sort.Invoke(toSort);
            for (int i = startIndex, k = 0; i <= endIndex; i++, k++) arr[i] = toSort[k];
            return arr;
        }
        public static T[] GetPart<T>(T[] arr, int startIndex, int endIndex) => arr.Skip(startIndex).Take(endIndex - startIndex + 1).ToArray();
        public static void PutPart<T>(T[] arr, T[] partArr, int index)
        {
            for (int i = index; i < partArr.Length + index; i++) arr[i] = partArr[i - index];
        }
        private static T[] IndexSort<T>(T[] arr, out int[] indexSwaps, Func<KeyValuePair<T, int>[], KeyValuePair<T, int>[]> sorter)
        {
            indexSwaps = new int[arr.Length];
            KeyValuePair<T, int>[] toSort = new KeyValuePair<T, int>[arr.Length];
            for (int i = 0; i < indexSwaps.Length; i++) toSort[i] = new KeyValuePair<T, int>(arr[i], i);
            toSort = sorter.Invoke(toSort);
            for (int i = 0; i < indexSwaps.Length; i++)
            {
                indexSwaps[toSort[i].Value] = i;
                arr[i] = toSort[i].Key;
            }
            return arr;
        }
        private static KeyValuePair<object, T>[] KVPConverter<T>(T[] arr)
        {
            KeyValuePair<object, T>[] outp = new KeyValuePair<object, T>[arr.Length];
            for (int i = 0; i < arr.Length; i++) outp[i] = new KeyValuePair<object, T>(null, arr[i]);
            return outp;
        }
        private static T[] KVPConverter<T>(KeyValuePair<object, T>[] kvp)
        {
            T[] outp = new T[kvp.Length];
            for (int i = 0; i < kvp.Length; i++) outp[i] = kvp[i].Value;
            return outp;
        }
        public static void KVPSplitter<T, V>(KeyValuePair<T, V>[] toSplit, out T[] arr1, out V[] arr2)
        {
            arr1 = new T[toSplit.Length];
            arr2 = new V[toSplit.Length];
            for (int i = 0; i < toSplit.Length; i++)
            {
                arr1[i] = toSplit[i].Key;
                arr2[i] = toSplit[i].Value;
            }
        }
        public static int[] GenArr(int length, int min = 0, int max = 10, Random rdm = default)
        {
            int[] outp = new int[length];
            if (rdm == default) rdm = new Random();
            for (int i = 0; i < outp.Length; i++) outp[i] = rdm.Next(min, max);
            return outp;
        }
        public static bool IsSorted<T,V>(KeyValuePair<T,V>[] arr, int startIndex = 0, int endIndex = -1) where V : IComparable<V>
        {
            if (endIndex == -1) endIndex = arr.Length - 1;
            for (int i = startIndex + 1; i <= endIndex; i++)
                if (arr[i - 1].Value.CompareTo(arr[i].Value) > 0)
                    return false;
            return true;
        }
        public static bool IsSorted<T>(T[] arr, int startIndex = 0, int endIndex = -1) where T : IComparable<T>
        {
            if (endIndex == -1) endIndex = arr.Length - 1;
            for (int i = startIndex + 1; i <= endIndex; i++) 
                if (arr[i - 1].CompareTo(arr[i]) > 0) 
                    return false;
            return true;
        }
        
        public static string TimeSortingAlgs(Func<int[], int[]> alg1, Func<int[], int[]> alg2, string name1 = "Alg1", string name2 = "Alg2")
        {
            int[] numsToSort = GenArr(1000, -1000, 1000), copy = new int[1000];
            Array.Copy(numsToSort, copy, 1000);
            int time1, time2, startTime;
            bool complete1, complete2;
            startTime = DateTime.Now.Millisecond;
            numsToSort = alg1.Invoke(numsToSort);
            time1 = DateTime.Now.Millisecond - startTime;
            complete1 = IsSorted(numsToSort);
            Array.Copy(copy, numsToSort, 1000);
            startTime = DateTime.Now.Millisecond;
            numsToSort = alg2.Invoke(numsToSort);
            time2 = DateTime.Now.Millisecond - startTime;
            complete2 = IsSorted(numsToSort);

            return name1 + "'s time was: " + time1 + "ms, and it did " + (complete1 ? "" : "not ") + "complete.\n" + name2 + "'s time was: " + time2 + "ms, and it did " + (complete2 ? "" : "not ") + "complete.";
        }
        public static string ToString<T>(IEnumerable<T> arr)
        {
            string outp = "[";
            foreach (T item in arr) outp += item + ", ";
            return outp.Substring(0, outp.Length - 2) + "]";
        }
        public static int[] StringToArr(string s)
        {
            string[] arr = s.Replace("[", "").Replace("]", "").Replace(", ", " ").Split(' ');
            int[] outp = new int[arr.Length];
            for (int i = 0; i < arr.Length; i++) outp[i] = int.Parse(arr[i]);
            return outp;
        }
    }
    public static class TextureLoader
    {
        public readonly static Dictionary<string, string[]> SavedContent;
        private readonly static List<KeyValuePair<string, Type>> LoadedPaths;
        public readonly static HashSet<UnloadedFile> AllFiles;
        public readonly static HashSet<LoadedFile<object>> FoundFiles; 
        public static bool HasLoaded { get; private set; }
        static TextureLoader()
        {
            AllFiles = new HashSet<UnloadedFile>();
            ReloadFoundFiles();
            HasLoaded = false;
            LoadedPaths = new List<KeyValuePair<string, Type>>();
            FoundFiles = new HashSet<LoadedFile<object>>();
            SavedContent = LoadTextFiles();
        }
        public static void ReloadFoundFiles()
        {
            AllFiles.Clear();
            AllFiles.UnionWith(FindAllFiles());
        }
        public static void ClearUsedPaths() => LoadedPaths.Clear();
        public static List<LoadedFile<T>> LoadContent<T> (ContentManager cm = null, bool searchAllFolders = false, string path = "")
        {
            if (cm == null) cm = GameHelper.GetDefault(cm);
            string originalPath = path;
            if (path.Length == 0) path = cm.RootDirectory;
            else path = cm.RootDirectory + "/" + path;
            List<LoadedFile<T>> outp = new List<LoadedFile<T>>();

            DirectoryInfo di = new DirectoryInfo(path);

            path = originalPath;
            if (path.Length > 0) path += "/";
            FileInfo[] arr = di.GetFiles("*.*");
            List<UnloadedFile> fi = new List<UnloadedFile>();
            if (searchAllFolders) fi = AllFiles.ToList();
            else for (int i = 0; i < arr.Length; i++) fi.Add(new UnloadedFile(arr[i], path + Path.GetFileNameWithoutExtension(arr[i].Name)));
            foreach (UnloadedFile uf in fi)
            {
                FileInfo f = uf.Info;
                if (FoundFiles.Any(a => a.Info.Equals(f))) continue;
                string hold = Path.GetFileNameWithoutExtension(f.Name);
                try
                {
                    LoadedFile<T> lf = uf.LoadFile(cm.Load<T>(""));
                    FoundFiles.Add(lf as LoadedFile<object>);
                    outp.Add(lf);
                }
                catch (Exception) { continue; }
            }
            return outp;
        }
        public static bool IsPathLoaded<T>(string path) => LoadedPaths.Contains(new KeyValuePair<string, Type>(path, typeof(T)));
        public static Dictionary<string, string[]> LoadTextFiles(bool searchAllFolders = true, string rootPath = "Content", string path = "")
        {
            if (path.Length == 0) path = rootPath;
            else path = rootPath + "/" + path;
            DirectoryInfo di = new DirectoryInfo(path);
            Dictionary<string, string[]> outp = new Dictionary<string, string[]>();

            List<FileInfo> fi = new List<FileInfo>(di.GetFiles("*.*"));
            if (searchAllFolders) fi = UnloadedFile.CreateList<FileInfo>(FindAllFiles(".txt"));
            foreach (FileInfo f in fi)
                outp[Path.GetFileNameWithoutExtension(f.Name)] = GameHelper.ReadFile(path + "/" + f.Name);
            return outp;
        }
        public static HashSet<UnloadedFile> FindAllFiles(string filter = "", string rootPath = "Content", string path = "")
        {
            if (path.Length > 0) path += "/";
            HashSet<UnloadedFile> outp = new HashSet<UnloadedFile>();
            List<DirectoryInfo> dirs = new List<DirectoryInfo> { new DirectoryInfo(rootPath + "/" + path) }, toAdd = new List<DirectoryInfo>();
            List<string> paths = new List<string> { path }, toAddPath = new List<string>();
            while (dirs.Count > 0)
            {
                for (int i = 0; i < dirs.Count; i++) 
                {
                    FileInfo[] arr = dirs[i].GetFiles("*.*");
                    foreach (FileInfo fi in arr)
                    {
                        if (filter.Length > 0 && !Path.GetExtension(fi.Name).Equals(filter)) continue;
                        if (paths[i].Length > 0) outp.Add(new UnloadedFile(fi, paths[i] + "/" + Path.GetFileNameWithoutExtension(fi.Name)));
                        else outp.Add(new UnloadedFile(fi, Path.GetFileNameWithoutExtension(fi.Name)));
                    }

                    DirectoryInfo[] arr2 = dirs[i].GetDirectories();
                    foreach (DirectoryInfo di in arr2)
                    {
                        toAdd.Add(di);
                        if (paths[i].Length > 0) toAddPath.Add(paths[i] + "/" + di.Name);
                        else toAddPath.Add(di.Name);
                    }
                }
                dirs.Clear();
                paths.Clear();
                dirs.AddRange(toAdd);
                paths.AddRange(toAddPath);
                toAdd.Clear();
                toAddPath.Clear();
            }
            return outp;
        }
        public static HashSet<UnloadedFile> FindAllFiles(string filter) => new HashSet<UnloadedFile>(AllFiles.Where(a => a.Info.Extension.Equals(filter)));
        public static Texture2D[] GetTexturesOfDirectory(string directory = "") => Array.ConvertAll(FoundFiles.Where(a => a.Name.Contains(directory)).Select(a => a.Data).ToArray(), a => a as Texture2D);
        public static void Draw(Dictionary<string, Texture2D> content, SpriteBatch sb = null, SpriteFont font = null, int size = 50, int gap = 50, int offset = 50)
        {
            if (sb == null) sb = GameHelper.GetDefault(sb);
            if (font == null) font = GameHelper.GetDefault(font);
            int x = offset, y = offset;
            foreach (string s in content.Keys)
            {
                sb.Draw(content[s], new Rectangle(x, y, size, size), Color.White);
                sb.DrawString(font, s, new Vector2(x + size / 2 - font.MeasureString(s).X / 2, y + size), Color.Black);
                x += size + gap;
                if (x + size >= GameHelper.Frame.Width)
                {
                    x = offset;
                    y += size + gap;
                }
            }
        }
        public static void Save(string fileName = GameHelper.TextureLoaderSaveName, string rootPath = "Content")
        {
            List<string> toSave = new List<string>();
            foreach (LoadedFile<object> file in FoundFiles)
            {
                Type T = file.Data.GetType();
                toSave.Add(file.Name + " | " + T.ToString() + " | " + T.Assembly.ToString());
                continue;
            }
            File.WriteAllLines(@rootPath + "/" + fileName + ".txt", toSave);
        }
        public static bool Load(ContentManager cm = null, string fileName = GameHelper.TextureLoaderSaveName, string rootPath = "")
        {
            if (cm == null) cm = GameHelper.GetDefault(cm);
            if (rootPath.Length < 1) rootPath = cm.RootDirectory;
            rootPath += "/";
            string[] data;
            try { data = GameHelper.ReadFile(rootPath + fileName + ".txt"); } catch (Exception) { return false; }
            List<UnloadedFile> allFiles = AllFiles.ToList();
            MethodInfo loader = typeof(ContentManager).GetMethod("Load");
            foreach (string s in data)
            {
                string[] spl = s.Split('|');
                string name = spl[0].Trim(), type = spl[1].Substring(1).Trim(), assem = spl[2].Substring(1);
                UnloadedFile uf;
                try { uf = UnloadedFile.FindFile(allFiles, name); } catch (Exception) { continue; }
                Type T = Type.GetType(type + ", " + assem);
                FoundFiles.Add(uf.LoadFile(loader.MakeGenericMethod(T).Invoke(cm, new object[] { name })));
            }
            HasLoaded = true;
            return true;
        }
        public static Dictionary<Type, FileInfo[]> FindFilesOfTypes(ContentManager cm = null, params Type[] types)
        {
            if (cm == null) cm = GameHelper.GetDefault(cm);
            Dictionary<Type, FileInfo[]> outp = new Dictionary<Type, FileInfo[]>();
            if (types.Length == 0) types = GameHelper.DefaultTypes;
            MethodInfo loader = typeof(ContentManager).GetMethod("Load"), converter = typeof(UnloadedFile).GetMethod("LoadFile");
            foreach (Type T in types) outp.Add(T, FindFilesOfType(cm, T, loader, converter));
            return outp;
        }
        private static FileInfo[] FindFilesOfType(ContentManager cm, Type toTest, MethodInfo loader, MethodInfo converter)
        {
            List<FileInfo> outp = new List<FileInfo>();
            foreach (UnloadedFile uf in AllFiles)
            {
                try
                {
                    object output = loader.MakeGenericMethod(toTest).Invoke(cm, new object[] { uf.Name });
                    outp.Add(uf.Info);
                    LoadedFile<object> lf = uf.LoadFile(output);
                    FoundFiles.Add(lf);
                }
                catch (Exception) { }
            }
            return outp.ToArray();
        }
        public static T GetName<T>(string fileName, bool isFullName = false)
        {
            foreach (LoadedFile<T> lf in FoundFiles)
            {
                if (lf == null) continue;
                if (!isFullName && lf.Name.Contains("/"))
                {
                    string[] temp = lf.Name.Split('/');
                    if (!fileName.Equals(temp[temp.Length - 1])) continue;
                }
                else if (!fileName.Equals(lf.Name)) continue;
                if (typeof(T) != lf.Data.GetType()) 
                    throw new FileNotFoundException("File Name given was not found for type given.");
                return lf.Data;
            }
            throw new FileNotFoundException("File Name given was not found for type given.");
        }
        public static string FindName<T>(T thing)
        {
            foreach (LoadedFile<T> lf in FoundFiles)
            {
                if (lf == null) continue;
                if (lf.Data.Equals(thing)) return lf.Name;
            }
            throw new Exception("Item not found.");
        } 
        public class LoadedFile<T>
        {
            public string Name { get; }
            public FileInfo Info { get; }
            public T Data { get; }
            public LoadedFile(FileInfo info, string name, T data)
            {
                Name = name;
                Info = info;
                Data = data;
            }
            public override string ToString() => $"{Name}: {Info}, {Data}";

            public static implicit operator LoadedFile<T>(LoadedFile<object> v) => typeof(T) == v.Data.GetType() ? new LoadedFile<T>(v.Info, v.Name, (T) v.Data) : null;
        }
        public class UnloadedFile
        {
            public string Name { get; }
            public FileInfo Info { get; }
            public UnloadedFile(FileInfo info, string name)
            {
                Name = name;
                Info = info;
            }
            public override string ToString() => $"{Name}: {Info}";
            public override bool Equals(object other) => other.GetType() == typeof(UnloadedFile) && ((UnloadedFile)other).Name == Name;
            public override int GetHashCode() => base.GetHashCode();
            public LoadedFile<T> LoadFile<T>(T data) => new LoadedFile<T>(Info, Name, data);

            public static List<T> CreateList<T>(IEnumerable<UnloadedFile> list)
            {
                List<T> outp = new List<T>();
                if (list.Count() == 0) return outp;
                if (typeof(T) == typeof(string)) foreach (UnloadedFile uf in list) outp.Add((T)(object)uf.Name);
                if (typeof(T) == typeof(FileInfo)) foreach (UnloadedFile uf in list) outp.Add((T)(object)uf.Info);
                if (outp.Count == 0) throw new ArgumentException("Generic given must be either a string or a FileInfo.");
                return outp;
            }
            public static UnloadedFile FindFile(IEnumerable<UnloadedFile> list, string name) => list.FirstOrDefault(a => a.Name == name);
        }
    }

    public class BinarySearchTree<T> where T : IComparable<T>
    {
        private BinarySearchTree<T> left, right;
        private T data;

        public BinarySearchTree() { }
        public BinarySearchTree(T item)
        {
            data = item;
        }
        public BinarySearchTree(params T[] items)
        {
            Add(items);
        }
        
        private BinarySearchTree<T> Next(BinarySearchTree<T> n, T item) => item.CompareTo(data) <= 0 ? n.left : n.right;
        private BinarySearchTree<T> GreatestParent(BinarySearchTree<T> n) => n.right == null || n.right.right == null ? n : GreatestParent(n.right);
        public void Add(T item)
        {
            Add(this, item);
        }
        public void Add(params T[] items)
        {
            if (items.Length == 0) return;
            data = items[0];
            for (int i = 1; i < items.Length; i++) Add(items[i]);
        }
        private void Add(BinarySearchTree<T> node, T item)
        {
            if (node == null) return;
            if (item.CompareTo(node.data) <= 0)
            {
                if (node.left == null) node.left = new BinarySearchTree<T>(item);
                else Add(node.left, item);
            } else
            {
                if (node.right == null) node.right = new BinarySearchTree<T>(item);
                else Add(node.right, item);
            }
        }
        public bool Remove(T item)
        {
            if (item.Equals(data))
            {
                if (left == null)
                {
                    if (right == null)
                    {
                        data = default;
                        return true;
                    }
                    data = right.data;
                    left = right.left;
                    right = right.right;
                    return true;
                }
                BinarySearchTree<T> gp = GreatestParent(left);
                if (left.right == null)
                {
                    data = left.data;
                    left = left.left;
                    return true;
                }
                data = gp.right.data;
                gp.right = null;
            }
            return Remove(null, this, item);
        }
        private bool Remove(BinarySearchTree<T> parent, BinarySearchTree<T> node, T item)
        {
            if (node == null) return false;
            if (node.data.Equals(item))
            {
                if (node.left != null)
                {
                    BinarySearchTree<T> leaf = GreatestParent(node.left);
                    if (node.left.right == null)
                    {
                        node.data = node.left.data;
                        node.left = node.left.left;
                        return true;
                    }
                    node.data = leaf.right.data;
                    leaf.right = null;
                    return true;
                }
                if (node.right == null)
                {
                    if (parent.data.CompareTo(item) > 0) parent.left = null;
                    else parent.right = null;
                    return true;
                }
            }
            return Remove(node, Next(node, item), item);
        }
        public bool Contains(T item)
        {
            return Contains(this, item);
        }
        private bool Contains(BinarySearchTree<T> node, T item)
        {
            if (node == null) return false;
            if (node.data.Equals(item)) return true;
            return Contains(Next(node, item), item);
        }
        public string InOrder()
        {
            string outp = "";
            if (left != null) outp = left.InOrder();
            outp += ", " + data.ToString();
            if (right != null) outp += right.InOrder();
            return outp;
        }
        public override string ToString() => "[" + InOrder().Substring(2) + "]";
        public override bool Equals(object obj) => obj != null && (obj.GetType() == typeof(BinarySearchTree<T>) && ((BinarySearchTree<T>)obj).data.Equals(data));
        public override int GetHashCode() //This was auto generated, I have no idea what hashCode does. (probably something to do with sorting in hashsets and hashmaps)
        {
            int hashCode = 113717442;
            hashCode = hashCode * -1521134295 + EqualityComparer<BinarySearchTree<T>>.Default.GetHashCode(left);
            hashCode = hashCode * -1521134295 + EqualityComparer<BinarySearchTree<T>>.Default.GetHashCode(right);
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(data);
            return hashCode;
        }
        public static bool operator ==(BinarySearchTree<T> tree1, BinarySearchTree<T> tree2) => (!(tree1 is null) && tree1.Equals(tree2)) || ((tree1 is null) && (tree2 is null));
        public static bool operator !=(BinarySearchTree<T> tree1, BinarySearchTree<T> tree2) => (!(tree1 is null) && !tree1.Equals(tree2)) || ((tree1 is null) && !(tree2 is null));

    }
    /**
     *<summary>
     *This class will split a sprite sheet up into it's components when given the color of the background.
     *</summary>
     */
    public class SheetReader : IUpdatable
    {
        public const int MinimumSize = 5; //The minimum size a sprite can be. (This counts as Width and Height, EX: 5 = at least 5x5 in size)
        public const int UpdatePriority = 0; //No update priority needed.

        public int GetUpdatePriority() => UpdatePriority;

        public readonly Texture2D Sheet;
        public readonly List<Rectangle> SpriteRects;
        public readonly List<Texture2D> Sprites;
        public readonly Dictionary<string, Texture2D> FileContents;
        private int scroll;
        private KeyboardState oldKb;

        //CONSTRUCTORS

        public SheetReader(Texture2D sheet, Color bgCol, params Color[] bannedCols)
        {
            scroll = 0;
            oldKb = Keyboard.GetState();
            Sheet = sheet;
            Sprites = new List<Texture2D>();
            SpriteRects = new List<Rectangle>();
            FileContents = null;
            SplitSheet(bgCol, bannedCols);
        }
        public SheetReader(params Color[] bannedCols)
        {
            scroll = 0;
            oldKb = Keyboard.GetState();
            Sheet = GameHelper.DefaultSpritesheet;
            Sprites = new List<Texture2D>();
            SpriteRects = new List<Rectangle>();
            FileContents = null;
            SplitSheet(default, bannedCols.Length == 0 ? null : bannedCols);
        }
        private SheetReader(Dictionary<string, Texture2D> dict, Dictionary<string, Rectangle> rectDict, Texture2D sheet)
        {
            scroll = 0;
            oldKb = Keyboard.GetState();
            FileContents = dict;
            Sheet = sheet;
            Sprites = new List<Texture2D>(dict.Values);
            SpriteRects = new List<Rectangle>(rectDict.Values);
        }

        //PRIVATE METHODS

        private void SplitSheet(Color bgCol = default, Color[] bannedStartingCols = null)
        {
            Sprites.Clear();
            Color[] pixels = new Color[Sheet.Width * Sheet.Height];
            Sheet.GetData(pixels);
            
            if (bannedStartingCols == null) bannedStartingCols = GameHelper.DefaultBannedColors;
            if (bgCol == default)
            {
                Console.WriteLine("No file found while loading! Trying to auto create sprites... (This may take a little bit)");
                MostCommonColor(pixels, out Dictionary<Color, int> common);
                KeyValuePair<Color, int>[] arr = Sorter.TimSort(common.ToArray());
                int c = arr.Length - 1;
                while (!SplitSheet(pixels, arr[c--].Key, bannedStartingCols)) { Sprites.Clear(); SpriteRects.Clear(); }
                Console.WriteLine("Complete!");
            }
            else SplitSheet(pixels, bgCol, bannedStartingCols);
        }
        private bool SplitSheet(Color[] pixels, Color bgCol, Color[] bannedStartingCols)
        {
            HashSet<Point> used = new HashSet<Point>();
            for (int pos = 0; pos < pixels.Length; pos++)
            {
                if (pixels[pos].Equals(bgCol)) continue;
                if (used.Contains(new Point(pos % Sheet.Width, pos / Sheet.Width))) continue;
                if (bannedStartingCols.Contains(pixels[pos])) continue;
                Rectangle hold = RipSprite(pos, pixels, bgCol);
                if (hold.Width * hold.Height < MinimumSize * MinimumSize) return false;
                Sprites.Add(RectToTxt(Sheet, hold));
                SpriteRects.Add(hold);
                used.UnionWith(Hitboxes.RectangleToPoints(hold));
            }
            return true;
        }
        private Rectangle RipSprite(int point, Color[] pixels, Color bgCol)
        {
            int width = 0, height = 0, maxWidth = Sheet.Width - point % Sheet.Width;
            int x = point % Sheet.Width, y = point / Sheet.Width;
            while (width < maxWidth && !pixels[point].Equals(bgCol))
            {
                width++;
                point++;
            }
            point--;
            while (point < pixels.Length && !pixels[point].Equals(bgCol))
            {
                height++;
                point += Sheet.Width;
            }
            return new Rectangle(x, y, width, height);
        }

        //PUBLIC METHODS

        public void Draw(SpriteBatch sb = null, SpriteFont font = null, int size = 50, int offset = 50, int gap = 25)
        {
            if (sb == null) sb = GameHelper.GetDefault(sb);
            if (font == null) font = GameHelper.GetDefault(font);
            int x = offset, y = offset - (size + gap) * scroll;
            if (FileContents != null) foreach (string s in FileContents.Keys)
                {
                    if (y + size < 0) goto skipDraw;
                    sb.Draw(FileContents[s], new Rectangle(x, y, size, size), Color.White);
                    sb.DrawString(font, s, new Vector2(x + size / 2 - font.MeasureString(s).X / 2, y + size), Color.Black);
                skipDraw:
                    x += size + gap;
                    if (x + size >= GameHelper.Frame.Width)
                    {
                        x = offset;
                        y += size + gap;
                    }
                }
            if (FileContents != null) return;
            for (int i = 0; i < Sprites.Count; i++)
            {
                if (y + size < 0) goto skipDraw;
                sb.Draw(Sprites[i], new Rectangle(x, y, size, size), Color.White);
                sb.DrawString(font, "" + i, new Vector2(x + size / 2 - font.MeasureString("" + i).X / 2, y + size), Color.Black);
            skipDraw:
                x += size + gap;
                if (x + size >= GameHelper.Frame.Width)
                {
                    x = offset;
                    y += size + gap;
                }
            }
        }
        public void Draw(string name, Rectangle boundingBox = default, SpriteBatch sb = default)
        {
            if (boundingBox == default) boundingBox = GameHelper.Frame;
            if (FileContents == null) return;
            if (sb == default) sb = GameHelper.GetDefault(sb);
            sb.Draw(FileContents[name], boundingBox, Color.White);
        }
        public void Draw(int index, Rectangle boundingBox = default, SpriteBatch sb = default)
        {
            if (boundingBox == default) boundingBox = GameHelper.Frame;
            if (sb == default) sb = GameHelper.GetDefault(sb);
            sb.Draw(Sprites[index], boundingBox, Color.White);
        }
        public void Update()
        {
            KeyboardState kb = Keyboard.GetState();

            if (kb.IsKeyDown(Keys.Down) && !oldKb.IsKeyDown(Keys.Down)) scroll++;
            if (kb.IsKeyDown(Keys.Up) && !oldKb.IsKeyDown(Keys.Up)) scroll--;

            if (scroll < 0) scroll = 0;

            oldKb = kb;
        }
        public void Save(string fileName = GameHelper.SheetReaderSaveName, string path = "", string names = "Rect")
        {
            string[] rects = GameHelper.RectanglesToString(SpriteRects.ToArray());
            for (int i = 0; i < rects.Length; i++) rects[i] = names + "_" + i + " | " + rects[i];
            File.WriteAllLines(GameHelper.DefaultCM.RootDirectory + "/" + path + (path.Length > 0 ? "/" : "") + fileName + ".txt", rects);
        }

        //PUBLIC STATIC METHODS

        public static SheetReader Load(Texture2D sheet = null, string fileName = GameHelper.SheetReaderSaveName)
        {
            if (sheet == null) sheet = TextureLoader.GetName<Texture2D>(GameHelper.DefaultSpriteSheetName);
            string[] data;
            try { data = TextureLoader.SavedContent[fileName]; }
            catch (Exception)
            {
                SheetReader toReturn = new SheetReader();
                if (GameHelper.AutoSave) toReturn.Save();
                return toReturn;
            }
            Dictionary<string, Texture2D> fileData = new Dictionary<string, Texture2D>();
            Dictionary<string, Rectangle> rectData = new Dictionary<string, Rectangle>();
            foreach (string s in data)
            {
                string name = s.Split('|')[0].Trim();
                string[] rect = s.Split('|')[1].Substring(1).Split(' ');
                Rectangle hold = new Rectangle(Int32.Parse(rect[0]), Int32.Parse(rect[1]), Int32.Parse(rect[2]), Int32.Parse(rect[3]));
                rectData.Add(name, hold);
                fileData.Add(name, RectToTxt(sheet, hold));
            }
            return new SheetReader(fileData, rectData, sheet);
        }
        public static Color MostCommonColor(Texture2D txt, out Dictionary<Color,int> common)
        {
            Color[] arr = new Color[txt.Width * txt.Height];
            txt.GetData(arr);
            return MostCommonColor(arr, out common);
        }
        public static Color MostCommonColor(Color[] cols, out Dictionary<Color, int> common)
        {
            common = new Dictionary<Color, int>();
            Color best = default;
            foreach (Color c in cols)
            {
                if (!common.ContainsKey(c)) common[c] = 0;
                common[c]++;
                if (best == default || common[best] < common[c]) best = c;
            }
            return best;
        }
        public static Color GetColFromSheet(Texture2D sheet, Point p)
        {
            Color[] pixels = new Color[sheet.Width * sheet.Height];
            sheet.GetData(pixels);
            return pixels[p.Y * sheet.Width + p.X];
        }
        public static Color GetColFromSheet(Color[] pixels, int txtWidth, Point p) => pixels[p.Y * txtWidth + p.X];
        public static Color[][] GetColsFromSheet(Texture2D sheet, Rectangle r)
        {
            HashSet<Point> points = Hitboxes.RectangleToPoints(r);
            Color[][] outp = new Color[r.Height][];
            Color[] pixels = new Color[sheet.Width * sheet.Height];
            sheet.GetData(pixels);
            for (int i = 0; i < outp.Length; i++) 
                outp[i] = new Color[r.Width];
            foreach (Point p in points)
                outp[p.Y - r.Y][p.X - r.X] = GetColFromSheet(pixels, sheet.Width, p);
            return outp;
        }
        public static Texture2D RectToTxt(Texture2D sheet, Rectangle r, GraphicsDevice gd = default)
        {
            if (gd == default) gd = GameHelper.GetDefault<GraphicsDeviceManager>().GraphicsDevice;
            Texture2D outp = new Texture2D(gd, r.Width, r.Height);
            Color[] pixels = GameHelper.TwoDimToOneDim(GetColsFromSheet(sheet, r));
            outp.SetData(pixels);
            return outp;
        }
        public static List<Texture2D> RectToTxt(Texture2D sheet, IEnumerable<Rectangle> rects, GraphicsDevice gd = default)
        {
            List<Texture2D> outp = new List<Texture2D>();
            if (gd == default) gd = GameHelper.GetDefault<GraphicsDeviceManager>().GraphicsDevice;
            foreach (Rectangle r in rects) outp.Add(RectToTxt(sheet, r, gd));
            return outp;
        }
    }
    /**
     * <summary>
     * This class holds hitboxes and can do collision quickly.
     * </summary>
     */
    public class Hitboxes
    {
        public const int DefaultUncertainty = 60;
        public const int DefaultUPC = 20; //UPC = uncertainty per color (aka per red, per green, per blue)
        public const int DefaultStray = DefaultUncertainty * 3 / (DefaultUPC / 10); //stray = how far away a color can get from the original wall color given.

        private readonly HashSet<Rectangle> hitboxes;
        private readonly HashSet<Point> Hitpoints;
        public readonly HashSet<Color> Colors, WallColors;
        public Rectangle BoundingBox { get; private set; }

        //CONSTRUCTORS

        public Hitboxes(Texture2D Image, Color WallCol, int uncertainty = DefaultUncertainty, int upc = DefaultUPC, int stray = DefaultStray)
            : this(Image, new Rectangle(0, 0, Image.Width, Image.Height), WallCol, uncertainty, upc, stray) { }
        public Hitboxes(Texture2D SpriteSheet, Rectangle Image, Color WallCol, int uncertainty = DefaultUncertainty, int upc = DefaultUPC, int stray = DefaultStray)
        {
            hitboxes = new HashSet<Rectangle>();
            Hitpoints = new HashSet<Point>();
            Colors = new HashSet<Color>();
            WallColors = new HashSet<Color>{ WallCol };
            GenerateHitboxes(SpriteSheet, Image, WallCol, uncertainty, upc, stray);
        }
        private Hitboxes(Rectangle boundingBox, IEnumerable<Rectangle> hitboxes, IEnumerable<Point> hitpoints)
        {
            BoundingBox = boundingBox;
            this.hitboxes = new HashSet<Rectangle>(hitboxes);
            this.Hitpoints = new HashSet<Point>(hitpoints);
        }

        //PRIVATE METHODS

        private void GenerateHitboxes(Texture2D SpriteSheet, Rectangle Image, Color WallCol, int uncertainty = DefaultUncertainty, int upc = DefaultUPC, int stray = DefaultStray)
        {
            if (SpriteSheet == null) SpriteSheet = GameHelper.DefaultSpritesheet;
            Color[] cols = new Color[SpriteSheet.Width * SpriteSheet.Height];
            SpriteSheet.GetData<Color>(cols);
            HashSet<Point> pointsOfIntrest = RectangleToPoints(Image), wallPoints = new HashSet<Point>();
            Point topLeft = new Point(SpriteSheet.Width, SpriteSheet.Height), bottomRight = new Point(0, 0);
            foreach (Point p in pointsOfIntrest)
            {
                Color point = cols[p.Y * SpriteSheet.Width + p.X];
                Colors.Add(point);
                if (ColorAboutEquals(point, WallColors, uncertainty, upc) && ColorAboutEquals(point, WallCol, stray))
                {
                    WallColors.Add(point);
                    wallPoints.Add(p);
                    if (p.X < topLeft.X) topLeft.X = p.X;
                    if (p.Y < topLeft.Y) topLeft.Y = p.Y;
                    if (p.X > bottomRight.X) bottomRight.X = p.X;
                    if (p.Y > bottomRight.Y) bottomRight.Y = p.Y;
                }
            }

            BoundingBox = new Rectangle(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);

            pointsOfIntrest.Clear();
            Hitpoints.UnionWith(wallPoints);
            while (wallPoints.Count > 0)
            {
                Point start = wallPoints.ElementAt(0);
                hitboxes.Add(CreateStrip(start, wallPoints));
            }

            CleanAndCombine();
        }
        private Rectangle CreateStrip(Point start, HashSet<Point> bounds)
        {
            int width = 0;
            while (bounds.Contains(start))
            {
                width++;
                bounds.Remove(start);
                start.X++;
            }
            return new Rectangle(start.X - width, start.Y, width, 1);
        }
        private void CleanAndCombine()
        {
            HashSet<Rectangle> toRemove = new HashSet<Rectangle>();
            List<Rectangle> hitboxRects = hitboxes.ToList();
            hitboxes.Clear();
            for (int i = 0; i < hitboxRects.Count; i++)
            {
                Rectangle r = hitboxRects[i];
                if (toRemove.Contains(r)) continue;
                foreach (Rectangle rr in hitboxRects)
                {
                    if (toRemove.Contains(rr)) continue;
                    if (r.X == rr.X && r.Width == rr.Width && r.Y + r.Height == rr.Y)
                    {
                        r.Height += rr.Height;
                        toRemove.Add(rr);
                    }
                }
                hitboxRects[i] = r;
            }
            foreach (Rectangle r in hitboxRects) 
                if (!toRemove.Contains(r)) 
                    hitboxes.Add(r);
        }
        private void ResetBoxAndPoints()
        {
            Hitpoints.Clear();
            foreach (Rectangle r in hitboxes) Hitpoints.UnionWith(RectangleToPoints(r));
            
            Point topLeft = new Point(GameHelper.Frame.Width, GameHelper.Frame.Height), bottomRight = new Point(0, 0);
            foreach (Point p in Hitpoints)
            {
                if (p.X < topLeft.X) topLeft.X = p.X;
                if (p.Y < topLeft.Y) topLeft.Y = p.Y;
                if (p.X > bottomRight.X) bottomRight.X = p.X;
                if (p.Y > bottomRight.Y) bottomRight.Y = p.Y;
            }
            BoundingBox = new Rectangle(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
        }
        private void Resize(double xFactor, double yFactor)
        {
            List<Rectangle> hitList = hitboxes.ToList();
            hitboxes.Clear();
            for (int i = 0; i < hitList.Count; i++)
            {
                Rectangle r = hitList[i];
                r.X = (int)Math.Round((double)r.X * xFactor);
                r.Y = (int)Math.Round((double)r.Y * yFactor);
                r.Width = (int)Math.Round((double)r.Width * xFactor);
                r.Height = (int)Math.Round((double)r.Height * yFactor);
                hitboxes.Add(r);
            }
            Rectangle main = BoundingBox;
            main.X = (int)Math.Round((double)main.X * xFactor);
            main.Y = (int)Math.Round((double)main.Y * yFactor);
            main.Width = (int)Math.Round((double)main.Width * xFactor);
            main.Height = (int)Math.Round((double)main.Height * yFactor);
            BoundingBox = main;
        }

        //PUBLIC METHODS

        public void Draw(Texture2D txt = default, Color c = default, SpriteBatch sb = default, bool drawBoundingBox = false, Color boundingBoxCol = default)
        {
            if (txt == null) txt = GameHelper.DefaultPixel;
            if (c == default) c = Color.Red;
            if (sb == null) sb = GameHelper.GetDefault(sb);
            if (drawBoundingBox)
            {
                if (boundingBoxCol.Equals(new Color())) return;
                sb.Draw(txt, BoundingBox, boundingBoxCol);
            }
            foreach (Rectangle r in hitboxes) sb.Draw(txt, r, c);
            
        }
        
        public void Resize(Rectangle r)
        {
            double xScale = (double)r.Width / (double)BoundingBox.Width;
            double yScale = (double)r.Height / (double)BoundingBox.Height;
            Resize(xScale, yScale);
            SetPosition(new Point(r.X, r.Y));
            ResetBoxAndPoints();
        }
        public void Resize(Rectangle r, int insets)
        {
            Resize(new Rectangle(r.X + insets, r.Y + insets, r.Width - insets * 2, r.Height - insets * 2));
        }
        public void SetPosition(Point pos)
        {
            int dx = pos.X - BoundingBox.X;
            int dy = pos.Y - BoundingBox.Y;
            Rectangle hold = BoundingBox;
            hold.X = pos.X;
            hold.Y = pos.Y;
            BoundingBox = hold;
            List<Rectangle> boxHit = hitboxes.ToList();
            hitboxes.Clear();
            for (int i=0;i<boxHit.Count;i++)
            {
                Rectangle r = boxHit[i];
                r.X += dx;
                r.Y += dy;
                hitboxes.Add(r);
            }
        }
        public void Save(string fileName = GameHelper.HitboxesSaveName, string path = "")
        {
            List<string> toWrite = new List<string> { GameHelper.RectangleToString(BoundingBox) };
            toWrite.AddRange(GameHelper.RectanglesToStrList(hitboxes.ToArray()));
            File.WriteAllLines(GameHelper.DefaultCM.RootDirectory + "/" + path + (path.Length > 0 ? "/" : "") + fileName + ".txt", toWrite);
        }
        public bool DoesCollideWith(Rectangle r)
        {
            HashSet<Point> rPoints = RectangleToPoints(r);
            rPoints.IntersectWith(Hitpoints);
            return rPoints.Count > 0;
        }
        public bool DoesCollideWith(Point p)
        {
            return Hitpoints.Contains(p);
        }
        public bool DoesCollideWith(Vector2 v)
        {
            return DoesCollideWith(new Point((int)v.X, (int)v.Y));
        }

        //PUBLIC STATIC METHODS

        public static HashSet<Point> RectangleToPoints(Rectangle r)
        {
            HashSet<Point> outp = new HashSet<Point>();
            for (int row = r.Y; row < r.Y + r.Height; row++)
                for (int col = r.X; col < r.X + r.Width; col++)
                    outp.Add(new Point(col, row));
            return outp;
        }
        public static HashSet<Point> RectanglesToPoints(IEnumerable<Rectangle> rects)
        {
            HashSet<Point> outp = new HashSet<Point>();
            foreach (Rectangle r in rects) outp.UnionWith(RectangleToPoints(r));
            return outp;
        }
        public static bool ColorAboutEquals(Color c1, Color c2, int uncertainty = DefaultUncertainty, int upc = DefaultUPC)
        {
            int difference = 0;
            difference += Math.Abs(c1.R - c2.R);
            if (uncertainty - difference < 0 || difference > upc) return false;
            difference += Math.Abs(c1.G - c2.G);
            if (uncertainty - difference < 0 || difference > upc * 2) return false;
            difference += Math.Abs(c1.B - c2.B);
            if (uncertainty - difference < 0) return false;
            return true;
        }
        public static bool ColorAboutEquals(Color c1, IEnumerable<Color> c2, int uncertainty = DefaultUncertainty, int upc = DefaultUPC)
        {
            foreach (Color c in c2) if (ColorAboutEquals(c1, c, uncertainty, upc)) return true;
            return false;
        }
        public static Hitboxes Load(string fileName = GameHelper.HitboxesSaveName)
        {
            string[] data;
            try { data = TextureLoader.SavedContent[fileName]; } catch (Exception) { return default; }
            string[] hold = data[0].Split(' ');
            Rectangle boundingBox = new Rectangle(Int32.Parse(hold[0]), Int32.Parse(hold[1]), Int32.Parse(hold[2]), Int32.Parse(hold[3]));
            List<Rectangle> hitboxes = new List<Rectangle>();

            for (int i = 1; i < data.Length; i++)
            {
                hold = data[i].Split(' ');
                hitboxes.Add(new Rectangle(Int32.Parse(hold[0]), Int32.Parse(hold[1]), Int32.Parse(hold[2]), Int32.Parse(hold[3])));
            }
            return new Hitboxes(boundingBox, hitboxes, RectanglesToPoints(hitboxes));
        }
        public static HashSet<Color> GetAllColors(Texture2D txt)
        {
            Color[] cols = new Color[txt.Width * txt.Height];
            txt.GetData(cols);
            return new HashSet<Color>(cols);
        }
        public static HashSet<Color> GetAllColors(Texture2D sheet, Rectangle pos)
        {
            HashSet<Point> points = RectangleToPoints(pos);
            HashSet<Color> outp = new HashSet<Color>();
            Color[] cols = new Color[sheet.Width * sheet.Height];
            sheet.GetData(cols);
            foreach (Point p in points) outp.Add(cols[p.Y * sheet.Width + p.X]);
            return outp;
        }
    }
}
