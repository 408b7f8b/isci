﻿using System;
using System.Linq;

namespace isci.Daten
{
    public class dtUInt8 : Dateneintrag
    {
        //public new System.Int32 value;

        public dtUInt8(System.Byte value, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.UInt8;
            this.value = value;
            if (path != "") this.path = path;
        }

        public override void LesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = reader.ReadByte();
            if (tmp != (System.Byte)value)
            {
                value = tmp;
                aenderung = true;
            }
        }

        public override void SchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((System.Byte)value);
        }

        public override string Serialisieren()
        {
            var s = value.ToString();
            return s;
        }

        public override void AusString(System.String s)
        {
            value = System.UInt16.Parse(s);
        }

        public override void AusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            value = token.ToObject<System.Byte>();
        }

        public static bool operator ==(dtUInt8 left, dtUInt8 right)
        {
            return (System.Byte)left.value == (System.Byte)right.value;
        }

        public static bool operator !=(dtUInt8 left, dtUInt8 right)
        {
            return (System.Byte)left.value != (System.Byte)right.value;
        }

        public static bool operator ==(dtUInt8 left, System.Byte right)
        {
            return (System.Byte)left.value == right;
        }

        public static bool operator !=(dtUInt8 left, System.Byte right)
        {
            return (System.Byte)left.value != right;
        }
    }
}