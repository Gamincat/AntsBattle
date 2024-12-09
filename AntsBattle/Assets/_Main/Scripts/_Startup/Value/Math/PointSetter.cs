using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PointSetter
{
    /// <summary>
    /// 指定した半径に、指定したサイズの物体を並べられるだけ並べる
    /// </summary>
    /// <param name="radian">半径</param>
    /// <param name="objectSize">オブジェクトのサイズ</param>
    /// <returns></returns>
    public static List<Vector3> CirclePointsForScale(float radian, float objectSize)
    {
        List<Vector3> poss = new List<Vector3>();
        var num = (int) ((2 * radian * Mathf.PI) / objectSize);
        poss.AddRange(CirclePoints(num, radian, 0f));

        return poss;
    }

    /// <summary>
    /// 指定したサイズのオブジェクトが指定した数収まるように円の半径を計算し、指定した角度を始点として塗りつぶすように配置する
    /// </summary>
    /// <param name="count">オブジェクトの数</param>
    /// <param name="startRadius">開始角度</param>
    /// <param name="objectSize">オブジェクトのサイズ</param>
    /// <param name="startRadian">中心の余白</param>
    /// <param name="widthLimit">これ以上横に広がらないようにする</param>
    /// <param name="backLimit">これ以上後ろに広がらないようにする</param>
    /// <returns></returns>
    public static List<Vector3> CircleFillPointsForCount(int count, float startRadius, float objectSize, float startRadian,
        float widthLimit = 0f, float backLimit = 0f)
    {
        List<Vector3> poss = new List<Vector3>();

        bool flipFlag = false;

        float radius = startRadius;
        while (count > 0)
        {
            var num = (int) ((2 * radius * Mathf.PI) / objectSize);
            count -= num;
            if (count < 0)
            {
                num += count;//紛らわしいけれど、この時countは必ずマイナスの値なので、numからはみ出たcountぶん削減する
                count = 0;
            }

            var newPoss = CirclePoints(num, radius, startRadian + (flipFlag ? 0.5f : 0f));

             LimitPoints(widthLimit, backLimit, ref newPoss, out var subtractCount);
             count += subtractCount;


            if (newPoss.Count <= 0)
            {
                flipFlag = !flipFlag;
                radius += (objectSize > 0f) ? objectSize : 1f;
            }
            else
            {
                poss.AddRange(newPoss); //15
                radius += objectSize;
            }
        }


        return poss;
    }

    /// <summary>
    /// 指定した範囲から出たポイントをリストから削除し、消した数を返す
    /// </summary>
    /// <param name="widthLimit">横幅制限</param>
    /// <param name="backLimit">後ろ制限</param>
    /// <param name="points">外部で用意された点群</param>
    /// <param name="subtractCount">減らした数</param>
    static void LimitPoints(float widthLimit, float backLimit, ref List<Vector3> points, out int subtractCount)
    {
        var prevCount = points.Count;

        if (widthLimit != 0f)
        {
            points.RemoveAll(b => b.x > widthLimit);
            points.RemoveAll(b => b.x < -widthLimit);
        }

        if (backLimit != 0f)
        {
            if (backLimit < 0) points.RemoveAll(b => b.z < backLimit);
            else points.RemoveAll(b => b.z > backLimit);
        }

        var currentCount = points.Count;

        subtractCount = prevCount - currentCount;
    }


    /// <summary>
    /// 指定した数の点を、指定した半径の長さに配置する
    /// </summary>
    /// <param name="n">数</param>
    /// <param name="len">半径の長さ</param>
    /// <param name="startRadian">配置の開始地点</param>
    /// <returns></returns>
    static List<Vector3> CirclePoints(int n, float len, float startRadian)
    {
        List<Vector3> poss = new List<Vector3>();

        var ofs = startRadian * (float) (Math.PI * 2f);
        for (int i = 0; i < n; i++)
        {
            var w = ((float) i / (float) n) * (Mathf.PI * 2f);
            var x = Mathf.Sin(w + ofs) * len;
            var z = Mathf.Cos(w + ofs) * len;
            poss.Add(new Vector3(x, 0f, z));
        }

        return poss;
    }


    public static List<Vector3> EmitCercle_RectFill(int count, float objectSize, bool backFlag, float widthLimit)
    {
        List<Vector3> poss = new List<Vector3>();

        var distanceSpeed = objectSize * (backFlag ? -1f : 1f);

        float distance = 0f;
        while (count > 0)
        {
            var num = (int) ((widthLimit * 2f) / objectSize);
//            if (num > count / 2) num = (count/2)+1;　尖らせる補正
            if (num < 1) num = 1;
            count -= num;
            if (count < 0)
            {
                num += count;
                count = 0;
            }

            var newPoss = LinePoints(num, distance, widthLimit);

            poss.AddRange(newPoss);
            distance += distanceSpeed;
        }


        return poss;
    }


    static List<Vector3> LinePoints(int num, float distance, float width)
    {
        var poss = new List<Vector3>();
        var w = (width * 2f) / num;
        for (int i = 0; i < num; i++)
        {
            var pos = new Vector3(-width + ((w * i) + (w * 0.5f)), 0f, distance);
            poss.Add(pos);
        }

        return poss;
    }
}
