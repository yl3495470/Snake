using Basic.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Basic.Net.Protocol
{
    public class PackageConverter
    {
        public static T PackageToObject<T>(ByteBuffer pkg)
        {
            return (T)_PackageToObject(pkg, typeof(T));
        }
        public static object PackageToObject(ByteBuffer pkg, Type type)
        {
            return _PackageToObject(pkg, type);
        }

        private static object _PackageToObject(ByteBuffer pkg, Type type)
        {
            object value = null;
            if (type == typeof(bool))
            {
                value = pkg.PopByte() > 0 ? true : false;
            }
            else if (type == typeof(sbyte))
            {
                value = pkg.PopSByte();
            }
            else if (type == typeof(byte))
            {
                value = pkg.PopByte();
            }
            else if (type == typeof(short))
            {
                value = pkg.PopShort();
            }
            else if (type == typeof(ushort))
            {
                value = pkg.PopUShort();
            }
            else if (type == typeof(int))
            {
                value = pkg.PopInt();
            }
            else if (type == typeof(uint))
            {
                value = pkg.PopUInt();
            }
            else if (type == typeof(long))
            {
                value = pkg.PopLong();
            }
            else if (type == typeof(ulong))
            {
                value = pkg.PopULong();
            }
            else if (type == typeof(string))
            {
                value = pkg.PopUTF();
            }
            else
            {
                Type subType = type.GetElementType();
                if (subType == null)
                {
                    value = System.Activator.CreateInstance(type);
                    FieldInfo[] fields = type.GetFields();
                    foreach (FieldInfo field in fields)
                    {
                        field.SetValue(value, _PackageToObject(pkg, field.FieldType));
                    }
                }
                else
                {
                    short count = pkg.PopShort();
                    if (subType == typeof(byte))
                    {
                        value = pkg.PopByteArray(count);
                    }
                    else
                    {
                        Array array = Array.CreateInstance(subType, count);
                        for (int i = 0; i < count; i++)
                        {
                            array.SetValue(_PackageToObject(pkg, subType), i);
                        }
                        value = array;
                    }
                }
            }
            return value;
        }

        public static ByteBuffer ObjectToPackage(object value, ByteBuffer pkg)
        {
            _ObjectToPackage(value, pkg);
            return pkg;
        }

        private static void _ObjectToPackage(object value, ByteBuffer pkg)
        {
            Type type = value.GetType();
            if (type == typeof(bool))
            {
                pkg.PushByte((bool)value == true ? (byte)1 : (byte)0);
            }
            else if (type == typeof(sbyte))
            {
                pkg.PushSByte((sbyte)value);
            }
            else if (type == typeof(byte))
            {
                pkg.PushByte((byte)value);
            }
            else if (type == typeof(short))
            {
                pkg.PushShort((short)value);
            }
            else if (type == typeof(ushort))
            {
                pkg.PushUShort((ushort)value);
            }
            else if (type == typeof(int))
            {
                pkg.PushInt((int)value);
            }
            else if (type == typeof(uint))
            {
                pkg.PushUInt((uint)value);
            }
            else if (type == typeof(long))
            {
                pkg.PushLong((long)value);
            }
            else if (type == typeof(ulong))
            {
                pkg.PushULong((ulong)value);
            }
            else if (type == typeof(string))
            {
                pkg.PushUTF((string)value);
            }
            else
            {
                Type subType = type.GetElementType();
                if (subType == null)
                {
                    FieldInfo[] fields = type.GetFields();
                    foreach (FieldInfo field in fields)
                    {
                        _ObjectToPackage(field.GetValue(value), pkg);
                    }
                }
                else
                {
                    Array array = value as Array;
                    short count = (short)array.Length;
                    pkg.PushShort(count);
                    for (int i = 0; i < count; i++)
                    {
                        _ObjectToPackage(array.GetValue(i), pkg);
                    }
                }
            }
        }

        ///======================强大的分割线========================
        ///以下内容用于处理动态接收数据

        /// <summary>
        /// 持续转换，即单次转换不一定转换完成
        /// </summary>
        /// <param name="pkg">数据流</param>
        /// <param name="type">对象类型</param>
        /// <param name="indexs">已转换的数据层次及位置，不能为空</param>
        /// <param name="root">已转换的数据对象</param>
        /// <returns></returns>
        public static bool StreamToObject(PackageIn pkg, Type type, List<int> indexs, object root, out object value)
        {
            return _StreamToObject(pkg, type, indexs, 0, root, out value);
        }

        private static bool _StreamToObject(PackageIn pkg, Type type, List<int> indexs, int depth, object entity, out object value)
        {
            value = null;
            if (type == typeof(bool))
            {
                if (!pkg.CanPopByte()) return false;
                value = pkg.PopByte() > 0 ? true : false;
            }
            else if (type == typeof(sbyte))
            {
                if (!pkg.CanPopByte()) return false;
                value = pkg.PopSByte();
            }
            else if (type == typeof(byte))
            {
                if (!pkg.CanPopByte()) return false;
                value = pkg.PopByte();
            }
            else if (type == typeof(short))
            {
                if (!pkg.CanPopShort()) return false;
                value = pkg.PopShort();
            }
            else if (type == typeof(ushort))
            {
                if (!pkg.CanPopShort()) return false;
                value = pkg.PopUShort();
            }
            else if (type == typeof(int))
            {
                if (!pkg.CanPopInt()) return false;
                value = pkg.PopInt();
            }
            else if (type == typeof(uint))
            {
                if (!pkg.CanPopInt()) return false;
                value = pkg.PopUInt();
            }
            else if (type == typeof(long))
            {
                if (!pkg.CanPopLong()) return false;
                value = pkg.PopLong();
            }
            else if (type == typeof(ulong))
            {
                if (!pkg.CanPopLong()) return false;
                value = pkg.PopULong();
            }
            else if (type == typeof(string))
            {
                if (!pkg.CanPopUTF()) return false;
                value = pkg.PopUTF();
            }
            else
            {
                bool subComplete = true;
                object subValue = null;
                Type subType = type.GetElementType();
                int count = 0;
                if (subType == null)
                {
                    if (entity != null)
                    {
                        value = entity;
                        FieldInfo[] fields = type.GetFields();
                        count = fields.Length;
                        for (int i = indexs[depth]; i < count; i++)
                        {
                            indexs[depth] = i;
                            subComplete = _StreamToObject(pkg, fields[i].FieldType, indexs, depth + 1, fields[i].GetValue(value), out subValue);
                            if (subValue != null) fields[i].SetValue(value, subValue);
                            if (!subComplete) break;
                        }
                    }
                    else
                    {
                        value = System.Activator.CreateInstance(type);
                        FieldInfo[] fields = type.GetFields();
                        count = fields.Length;
                        indexs.Add(0);
                        for (int i = 0; i < count; i++)
                        {
                            indexs[depth] = i;
                            subComplete = _StreamToObject(pkg, fields[i].FieldType, indexs, depth + 1, null, out subValue);
                            if (subValue != null) fields[i].SetValue(value, subValue);
                            if (!subComplete) break;
                        }
                    }
                }
                else
                {
                    if (entity != null)
                    {
                        Array array = entity as Array;
                        count = array.Length;
                        for (int i = indexs[depth]; i < count; i++)
                        {
                            indexs[depth] = i;
                            subComplete = _StreamToObject(pkg, subType, indexs, depth + 1, array.GetValue(i), out subValue);
                            array.SetValue(subValue, i);
                            if (!subComplete) break;
                        }
                        value = array;
                    }
                    else
                    {
                        if (!pkg.CanPopShort()) return false;
                        count = pkg.PopShort();
                        Array array = Array.CreateInstance(subType, count);
                        indexs.Add(0);
                        for (int i = 0; i < count; i++)
                        {
                            indexs[depth] = i;
                            subComplete = _StreamToObject(pkg, subType, indexs, depth + 1, null, out subValue);
                            array.SetValue(subValue, i);
                            if (!subComplete) break;
                        }
                        value = array;
                    }
                }
                if (subComplete)
                {
                    indexs.RemoveAt(indexs.Count - 1);
                }
                return subComplete;
            }
            return true;
        }
    }
}
