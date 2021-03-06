// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: clientDataPos.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from clientDataPos.proto</summary>
public static partial class ClientDataPosReflection {

  #region Descriptor
  /// <summary>File descriptor for clientDataPos.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static ClientDataPosReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "ChNjbGllbnREYXRhUG9zLnByb3RvIjsKDWNsaWVudERhdGFQb3MSDQoFSW5k",
          "ZXgYASABKBESDgoGTGVuZ3RoGAIgASgREgsKA1BvcxgDIAEoESI0ChNjbGll",
          "bnREYXRhUG9zQ29uZmlnEh0KBURhdGFzGAEgAygLMg4uY2xpZW50RGF0YVBv",
          "c0ICSANiBnByb3RvMw=="));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { },
        new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::clientDataPos), global::clientDataPos.Parser, new[]{ "Index", "Length", "Pos" }, null, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::clientDataPosConfig), global::clientDataPosConfig.Parser, new[]{ "Datas" }, null, null, null, null)
        }));
  }
  #endregion

}
#region Messages
public sealed partial class clientDataPos : pb::IMessage<clientDataPos>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , pb::IBufferMessage
#endif
{
  private static readonly pb::MessageParser<clientDataPos> _parser = new pb::MessageParser<clientDataPos>(() => new clientDataPos());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<clientDataPos> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::ClientDataPosReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public clientDataPos() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public clientDataPos(clientDataPos other) : this() {
    index_ = other.index_;
    length_ = other.length_;
    pos_ = other.pos_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public clientDataPos Clone() {
    return new clientDataPos(this);
  }

  /// <summary>Field number for the "Index" field.</summary>
  public const int IndexFieldNumber = 1;
  private int index_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int Index {
    get { return index_; }
    set {
      index_ = value;
    }
  }

  /// <summary>Field number for the "Length" field.</summary>
  public const int LengthFieldNumber = 2;
  private int length_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int Length {
    get { return length_; }
    set {
      length_ = value;
    }
  }

  /// <summary>Field number for the "Pos" field.</summary>
  public const int PosFieldNumber = 3;
  private int pos_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int Pos {
    get { return pos_; }
    set {
      pos_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as clientDataPos);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(clientDataPos other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (Index != other.Index) return false;
    if (Length != other.Length) return false;
    if (Pos != other.Pos) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (Index != 0) hash ^= Index.GetHashCode();
    if (Length != 0) hash ^= Length.GetHashCode();
    if (Pos != 0) hash ^= Pos.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    output.WriteRawMessage(this);
  #else
    if (Index != 0) {
      output.WriteRawTag(8);
      output.WriteSInt32(Index);
    }
    if (Length != 0) {
      output.WriteRawTag(16);
      output.WriteSInt32(Length);
    }
    if (Pos != 0) {
      output.WriteRawTag(24);
      output.WriteSInt32(Pos);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
    if (Index != 0) {
      output.WriteRawTag(8);
      output.WriteSInt32(Index);
    }
    if (Length != 0) {
      output.WriteRawTag(16);
      output.WriteSInt32(Length);
    }
    if (Pos != 0) {
      output.WriteRawTag(24);
      output.WriteSInt32(Pos);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(ref output);
    }
  }
  #endif

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (Index != 0) {
      size += 1 + pb::CodedOutputStream.ComputeSInt32Size(Index);
    }
    if (Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeSInt32Size(Length);
    }
    if (Pos != 0) {
      size += 1 + pb::CodedOutputStream.ComputeSInt32Size(Pos);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(clientDataPos other) {
    if (other == null) {
      return;
    }
    if (other.Index != 0) {
      Index = other.Index;
    }
    if (other.Length != 0) {
      Length = other.Length;
    }
    if (other.Pos != 0) {
      Pos = other.Pos;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    input.ReadRawMessage(this);
  #else
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          Index = input.ReadSInt32();
          break;
        }
        case 16: {
          Length = input.ReadSInt32();
          break;
        }
        case 24: {
          Pos = input.ReadSInt32();
          break;
        }
      }
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
          break;
        case 8: {
          Index = input.ReadSInt32();
          break;
        }
        case 16: {
          Length = input.ReadSInt32();
          break;
        }
        case 24: {
          Pos = input.ReadSInt32();
          break;
        }
      }
    }
  }
  #endif

}

public sealed partial class clientDataPosConfig : pb::IMessage<clientDataPosConfig>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , pb::IBufferMessage
#endif
{
  private static readonly pb::MessageParser<clientDataPosConfig> _parser = new pb::MessageParser<clientDataPosConfig>(() => new clientDataPosConfig());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<clientDataPosConfig> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::ClientDataPosReflection.Descriptor.MessageTypes[1]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public clientDataPosConfig() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public clientDataPosConfig(clientDataPosConfig other) : this() {
    datas_ = other.datas_.Clone();
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public clientDataPosConfig Clone() {
    return new clientDataPosConfig(this);
  }

  /// <summary>Field number for the "Datas" field.</summary>
  public const int DatasFieldNumber = 1;
  private static readonly pb::FieldCodec<global::clientDataPos> _repeated_datas_codec
      = pb::FieldCodec.ForMessage(10, global::clientDataPos.Parser);
  private readonly pbc::RepeatedField<global::clientDataPos> datas_ = new pbc::RepeatedField<global::clientDataPos>();
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public pbc::RepeatedField<global::clientDataPos> Datas {
    get { return datas_; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as clientDataPosConfig);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(clientDataPosConfig other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if(!datas_.Equals(other.datas_)) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    hash ^= datas_.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    output.WriteRawMessage(this);
  #else
    datas_.WriteTo(output, _repeated_datas_codec);
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
    datas_.WriteTo(ref output, _repeated_datas_codec);
    if (_unknownFields != null) {
      _unknownFields.WriteTo(ref output);
    }
  }
  #endif

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    size += datas_.CalculateSize(_repeated_datas_codec);
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(clientDataPosConfig other) {
    if (other == null) {
      return;
    }
    datas_.Add(other.datas_);
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    input.ReadRawMessage(this);
  #else
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 10: {
          datas_.AddEntriesFrom(input, _repeated_datas_codec);
          break;
        }
      }
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
          break;
        case 10: {
          datas_.AddEntriesFrom(ref input, _repeated_datas_codec);
          break;
        }
      }
    }
  }
  #endif

}

#endregion


#endregion Designer generated code
