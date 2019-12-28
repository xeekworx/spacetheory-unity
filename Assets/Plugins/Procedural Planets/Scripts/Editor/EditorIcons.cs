using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPlanets
{
    /// <summary>
    /// Editor icons as Base64 encoded PNG.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>
    
    public static class EditorIcons
    {

        [SerializeField] static Texture2D _iconZoom;
        [SerializeField] static Texture2D _iconEdit;
        [SerializeField] static Texture2D _iconDelete;
        [SerializeField] static Texture2D _iconHelp;
        [SerializeField] static Texture2D _iconArrowUp;
        [SerializeField] static Texture2D _iconArrowDown;
        [SerializeField] static Texture2D _iconPlusInCircle;

        public static Texture2D IconZoom()
        {
            if (_iconZoom == null)
                _iconZoom = IconFromBase64("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAABNElEQVQ4EZ2SsUoDQRCGNzYSwV4sfYAUYiOkVEsbNQjaRAWfx0IbsRAFsbKxEFIJPkACYi1qwMZCBEUSOL8/7ixzy4nowHfz78y/c3d7F4qiCI4N9BW8wAfcwR7MgPclbaKG4RJ+iiGNdTB/yiY6bucBegnmYRf0FBZNhO0ZZV1WrUtezg1xfR09/byvATexuZ833XoS/Rl9C64exkIIc6A4/E6V1zeqndhpeocGjMfCq29U6H6sTfieBjzFQsM3KrTd+b7U43106ooelE7YrVdkIN5hGpJPQgWLC8QUJAO6ZU3yadYLZlxzpgFag46h6+qSz7AFti8NUEE/zy1UxVlW3GY9GqJfuHQmLBZhFurwAPp8j7AJJ2CxgzhKj2ITf8m6s4/2XwfIXxrynwHa0wYd6PkX6KWR4tvQWrQAAAAASUVORK5CYII=");

            return _iconZoom;
        }

        public static Texture2D IconEdit()
        {
            if (_iconEdit == null)
                _iconEdit = IconFromBase64("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAA9UlEQVQ4EZWRMQrCQBBFNcY0NlZ2XsDeUwgeQDyBHsBTiLW1ouAFLGwsgohgbSmICLmAlRDfD9mwmhiTgZfZGf6f2SSVMAwrJfDQ7uEBPfmqepSIAG3L0veLbm+wqA5z+IgiA2o4bnAA6SdgYv1vgMwnoyZvQB7dZKdz3gAHgW2mjOLMM/E51gexj+ofoWs34/OT7CV9e1p8/rVZ6xff+uQqBczLb7Nqe4BLI+udaac3yyzMgCZFAFmRuRnhxwAVfoY79c7GaLKMI5jFjRXZRO5mRNENXH5HB8ZwhwG04QVD+B9MupiV5KmZXDTrBoot+HBVUSbeiSWGcddDUkQAAAAASUVORK5CYII=");

            return _iconEdit;
        }

        public static Texture2D IconDelete()
        {
            if (_iconDelete == null)
                _iconDelete = IconFromBase64("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAA6UlEQVQ4EY1TWwrCQAxcvJCI1MOJ2kMovj6kPk6mp7Bi68xiytgmYGBImkwe3d2ktm0TMAbuwPn7TV+EE2IPYEIOSVPgCZjsYETJGyNB10BBoidbOPtF1h6RpIsXgE8n0c5Kv1kXkj1ZwDn3AvAdgXwGVoRj/ysHEHOeJZuOJtHCuXNUgIWWyu7ZJb6tWdajNJT30NV5ms4yo1cxOm0dRG/n5xDde9ZMsbt3Yv8TdS6RtJJENfMkLMC37YmednQ7FQt4sofTpjPtvhMGC6CWKtrZkk3rJC/kzCzA1eQ6XwHzRboCh+vMLU4fIv9cCgenB+UAAAAASUVORK5CYII=");
            return _iconDelete;
        }

        public static Texture2D IconHelp()
        {
            if (_iconHelp == null)
                _iconHelp = IconFromBase64("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAA1ElEQVQ4EcVSbRXCMAzseBiYBZAwC7MwC7OABSyAFiwgYZOAhXIXSJqV5hc/lveu+bzrXrOUc04VBuRXoLYbChOwmd8kaM41q5FTyHgWoHhqDEclXiTcQyo2l1CiFecZ6ICnVMphs15gKn2J7jgpQnt8nJ29RkcN4EmgDQDF9NYR8QXwpr3k36AVR4/KTcl8i6S1iDwqmV6Ha9+j+QK8LUjsZsTCqYma84fxRjGuWfvm/Rb8I9krf4vcgm7Ez6WOqv9Y9AVcG5UVS3RJJBDN/9T3F3gDjvOOOWZcGO4AAAAASUVORK5CYII=");
            return _iconHelp;
        }

        public static Texture2D IconArrowUp()
        {
            if (_iconArrowUp == null)
                _iconArrowUp = IconFromBase64("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAqklEQVQ4EbWSWQ7CMAxEU67QU7AcoCy34fAUfihcIMxDtpRIrvIRNdLUjmdJFHXIOaeetesx49084KZDwPriDVZw1fxroA914VBiDB/hYaAPQ6KAi5ln1b2BnhC4ylNtTLCoYjgUYnpmcFVIGXA2wVO1NLuGGRwhaP9zJycjXqpHJ4MKh4YQPImAUXgLLbMf5iF4Rh/etTkJvm9VtHjSwKdnbf4rNy/XfYMf07uivukykSIAAAAASUVORK5CYII=");
            return _iconArrowUp;
        }

        public static Texture2D IconArrowDown()
        {
            if (_iconArrowDown == null)
                _iconArrowDown = IconFromBase64("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAtElEQVQ4Ec2T2Q3CMBBEHVpIFRwFcHVDq/QS4IejATPPWlubQIxQPiDSxOvZeSs7UpoYY5jyzKbAsP8z4KDTrL64DlmYEPQRW+kqXaSlhFcTGbIwbQ6utblZozYkw2Rh0gnykI0NOWtd0BwIjx4w2dQfhrYWOGn1Q6jxgMkUrhTOJHCXAOYmarwerH3vCn7YzoBOKwLG85lUvxguBPAwvYXJNrwqz956x7HMpwFjXPF//y88AZmbdnQbPFuBAAAAAElFTkSuQmCC");
            return _iconArrowDown;
        }

        public static Texture2D IconPlusInCircle()
        {
            if (_iconPlusInCircle == null)
                _iconPlusInCircle = IconFromBase64("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAABXUlEQVQ4EXWTPS9EURCG77KI6LYgNGR9FIoVEQqNbn+PXyD+gUJHhaisKHxGQjZBodYtQaL0ETosOZ73OLO59+TuJM/OzDvvmZzdnE2cc0nEFP0aPIPFO8UGzEHGn2kYroDFB8UpHMOLieR1aJ1rFYhHwVQnz6ZNoa6QD0BxCf6sLVj18v8NTGuXl4N3W0tkGgvCtYSIMv1EpMlzAoppNZu+dG6EHC94RPvM0UvhzKEOfMFbjkkzLWi2mT2hu44kSbrhAizKFLfQgCGQR7W0ClicqShal8rS+kPfSS7AQOi7QlZq+ppb/MAr6MoxD2h5v4F8d+C/Qo1NJRj0G7MfvbQ9Wcl3fXyOQl2bJrWJOIf4Bno8Mzn6Dppi3g7onSuWwLR2edE7nduXN226CoMaeVzDiGF6ezM31AXNY5MZmLl70HPdggZY7FIUwZ+NF6hfgD34BotfCv2RqpA58weoTL4ZPITRNAAAAABJRU5ErkJggg==");
            return _iconPlusInCircle;
        }

        public static Texture2D IconFromBase64(string _base64String)
        {
            Texture2D _icon = new Texture2D(1, 1);
            byte[] _bytes = System.Convert.FromBase64String(_base64String);
            _icon.LoadImage(_bytes);
            if (UnityEditor.EditorGUIUtility.isProSkin)
            {
                return _icon;
            }
            else
            {
                return InvertColors(_icon);
            }
            
        }

        public static Texture2D InvertColors(Texture2D _sourceTex)
        {
            Texture2D _destTex = new Texture2D(_sourceTex.width, _sourceTex.height, TextureFormat.ARGB32, false);
            for (int _y = 0; _y < _sourceTex.height; _y++)
            {
                for (int _x = 0; _x < _sourceTex.height; _x++)
                {
                    Color _c = _sourceTex.GetPixel(_x, _y);
                    _destTex.SetPixel(_x, _y, new Color(1.0f - _c.r, 1.0f - _c.g, 1.0f - _c.b, _c.a));
                }
            }
            _destTex.Apply();
            return _destTex;
        }

    }

}
