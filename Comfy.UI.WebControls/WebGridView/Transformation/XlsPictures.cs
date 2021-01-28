

using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
namespace Comfy.UI.WebControls.Transformation
{
	#region MsoDataTypes
	public struct MsoHeader {
		public static int Size() {
			return sizeof(uint) + 2 * sizeof(ushort);
		}
		public ushort Options;
		public ushort FBT;
		public uint Len;
		public void WriteToStream(XlsStream stream) {
			stream.Write(Options);
			stream.Write(FBT);
			stream.Write((int)Len);
		}
	}
	public struct BIFFHeader {
		public static int Size() {
			return 2 * sizeof(ushort); 
		}
		public ushort RecId;
		public ushort Length;
		public void WriteToStream(XlsStream stream) {
			stream.WriteCore(RecId);
			stream.WriteCore(Length);
		}
	}
	public abstract class MsoData : IDisposable {
		ushort version;
		protected ushort fInstance;
		public MsoData(ushort version, ushort instance) {
			this.version = version;
			this.fInstance = instance;
		}
		public ushort Version { get { return version; } }
		public ushort Instance { get { return fInstance; } }
		public int Size { get { return GetSize(); } }
		public uint DataSize { get { return Convert.ToUInt32(Size - MsoHeader.Size()); }  }
		protected void WriteMsoHeader(ushort FBT, uint len, XlsStream stream) {
			MsoHeader header = new MsoHeader();
			header.Options = Convert.ToUInt16(version + (fInstance << 4));
			header.FBT = FBT;
			header.Len = len;
			header.WriteToStream(stream);
		}
		protected abstract int GetSize();
		public abstract void Write(XlsStream stream);
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~MsoData() {
			Dispose(false);
		}
		#endregion
	}
	public struct IdClust {
		public uint sGOwningSpIds;
		public uint spIdsUsed;
	}
	public struct MsoDggRec {
		public static int Size() {
			return 6 * 4;
		}
		public uint maxSpId;
		public uint idClustCount;
		public uint shapesSaved;
		public uint drawingsSaved;
		public IdClust idClust;
	}
	public class MsoDataDgg : MsoData {
		int picsInSheets;
		public MsoDataDgg(ushort version, ushort instance, int picsInSheets) : base(version, instance) {
			this.picsInSheets = picsInSheets;
		}
		protected override int GetSize() { 
			return MsoDggRec.Size() + MsoHeader.Size();
		}
		public override void Write(XlsStream stream) {
			MsoDggRec rec = new MsoDggRec();
			rec.maxSpId = Convert.ToUInt32(XlsConsts.MsoSpId + picsInSheets);
			rec.idClustCount = 2;
			rec.shapesSaved = Convert.ToUInt32(picsInSheets + 1);
			rec.drawingsSaved = 1;
			rec.idClust.sGOwningSpIds = 1;
			rec.idClust.spIdsUsed = Convert.ToUInt32(picsInSheets + 1);
			WriteMsoHeader(XlsConsts.MsoDgg, DataSize, stream);
			stream.Write(rec.maxSpId);
			stream.Write(rec.idClustCount);
			stream.Write(rec.shapesSaved);
			stream.Write(rec.drawingsSaved);
			stream.Write(rec.idClust.sGOwningSpIds);
			stream.Write(rec.idClust.spIdsUsed);
		}
	}
	
	public struct MsoDgRec {
		public static int Size() {
			return 2 * sizeof(uint);
		}
		public uint shapes;
		public uint spIdCurr;
	}
	
	public class MsoDataDg : MsoData {
		public MsoDataDg(ushort version, ushort instance) : base(version, instance) {
		}
		protected override int GetSize() { 
			return MsoDgRec.Size() + MsoHeader.Size();
		}
		public override void Write(XlsStream stream) {
			MsoDgRec rec = new MsoDgRec();
			rec.shapes = 2;
			rec.spIdCurr = XlsConsts.MsoSpId;
			WriteMsoHeader(XlsConsts.MsoDg, DataSize, stream);
			stream.Write(rec.shapes);
			stream.Write(rec.spIdCurr);
		}
	}
	
	public class MsoDataSpGr : MsoData {
		public const int DataLength = 16;
		byte[] spGrData = new byte[DataLength];
		public MsoDataSpGr(ushort version, ushort instance) : base(version, instance) {
		}
		protected override int GetSize() { 
			return DataLength + MsoHeader.Size();
		}
		public override void Write(XlsStream stream) {
			WriteMsoHeader(XlsConsts.MsoSpGr, DataSize, stream);
			stream.Write(spGrData, 0, DataLength);
		}
	}
	
	public struct MsoSpRec {
		public static int Size() {
			return 2 * sizeof(uint);
		}
		public uint spId;
		public uint flags;
	}
	
	public class MsoDataSp : MsoData {
		uint spId;
		ushort flags;
		public MsoDataSp(ushort version, ushort instance, uint spId, ushort flags) : base(version, instance) {
			this.spId = spId;
			this.flags = flags;
		}
		protected override int GetSize() { 
			return MsoSpRec.Size() + MsoHeader.Size();
		}
		public override void Write(XlsStream stream) {
			MsoSpRec rec = new MsoSpRec();
			rec.spId = this.spId;
			rec.flags = this.flags;
			WriteMsoHeader(XlsConsts.MsoSp, DataSize, stream);
			stream.Write(rec.spId);
			stream.Write(rec.flags);
		}
	}
	
	public class MsoDataClientData : MsoData {
		public MsoDataClientData(ushort version, ushort instance) : base(version, instance) {
		}
		protected override int GetSize() { 
			return MsoHeader.Size();
		}
		public override void Write(XlsStream stream) {
			WriteMsoHeader(XlsConsts.MsoClientData, DataSize, stream);
		}
	}
	
	public struct MsoAnchorRec {
		public static int Size() {
			return 9 * 2;
		}
		public ushort options;
		public ushort col1;
		public ushort col1Offset;
		public ushort row1;
		public ushort row1Offset;
		public ushort col2;
		public ushort col2Offset;
		public ushort row2;
		public ushort row2Offset;
	}
	
	public class AhchorObject {
		ushort row1;
		ushort col1;
		ushort row1Offset;
		ushort col1Offset;
		ushort row2;
		ushort col2;
		ushort row2Offset;
		ushort col2Offset;
		ushort itemId;
		public AhchorObject() {
		}
		public ushort Row1 { get { return row1; } set { row1 = value; } }
		public ushort Col1 { get { return col1; } set { col1 = value; } }
		public ushort Row1Offset { get { return row1Offset; } set { row1Offset = value; } }
		public ushort Col1Offset { get { return col1Offset; } set { col1Offset = value; } }
		public ushort Row2 { get { return row2; } set { row2 = value; } }
		public ushort Col2 { get { return col2; } set { col2 = value; } }
		public ushort Row2Offset { get { return row2Offset; } set { row2Offset = value; } }
		public ushort Col2Offset { get { return col2Offset; } set { col2Offset = value; } }
		public ushort ItemId { get { return itemId; } set { itemId = value; } }
	}
	
	public class MsoDataClientAnchor : MsoData {
		AhchorObject anchor;
		ushort options;
		public MsoDataClientAnchor(ushort version, ushort instance, ushort options, AhchorObject anchor) : base(version, instance) {
			this.anchor = anchor;
			this.options = options;
		}
		protected override int GetSize() { 
			return MsoAnchorRec.Size() + MsoHeader.Size();
		}
		public override void Write(XlsStream stream) {
			MsoAnchorRec rec = new MsoAnchorRec();
			rec.options = options;
			rec.col1 = anchor.Col1;
			rec.col1Offset = anchor.Col1Offset;
			rec.row1 = anchor.Row1;
			rec.row1Offset = anchor.Row1Offset;
			rec.col2 = anchor.Col2;
			rec.col2Offset = anchor.Col2Offset;
			rec.row2 = anchor.Row2;
			rec.row2Offset = anchor.Row2Offset;
			WriteMsoHeader(XlsConsts.MsoClientAnchor, DataSize, stream);
			stream.Write(rec.options);
			stream.Write(rec.col1);
			stream.Write(rec.col1Offset);
			stream.Write(rec.row1);
			stream.Write(rec.row1Offset);
			stream.Write(rec.col2);
			stream.Write(rec.col2Offset);
			stream.Write(rec.row2);
			stream.Write(rec.row2Offset);
		}
	}
	
	public struct OptData {
		public ushort options;
		public uint value;
		public int pdata;
	}
	
	public struct OptValue {
		public static int Size() {
			return 2 + 4;
		}
		public ushort options;
		public uint value;
	}
	
	public class MsoDataOpt : MsoData {
		List<OptData> values;
		public MsoDataOpt(ushort version, ushort instance) : base(version, instance) {
			values = new List<OptData>();
		}
		public void AddValue(ushort id, uint val) {
			OptData item = new OptData();
			item.options = id;
			item.value = val;
			values.Add(item);
		}
		protected override int GetSize() { 
			int size = MsoHeader.Size();
			for(int i = 0; i < values.Count; i++) {
				size += OptValue.Size();
				OptData item = (OptData)values[i];
			}
			return size;
		}
		public override void Write(XlsStream stream) {
			fInstance = Convert.ToUInt16(values.Count);
			WriteMsoHeader(XlsConsts.MsoOpt, DataSize, stream);
			for(int i = 0; i < values.Count; i++) {
				OptData item = values[i];
				stream.Write(item.options);
				stream.Write(item.value);
			}
		}
	}
	
	public class MsoDataBLIP : MsoData {
		static Hashtable blipTypes;
		XlsPicture picture;
		static MsoDataBLIP() {
			blipTypes = new Hashtable();
			blipTypes[XlsPictureType.Jpeg] = XlsConsts.BLIP_Jpeg;
			blipTypes[XlsPictureType.Png] = XlsConsts.BLIP_Png;
		}
		public MsoDataBLIP(ushort version, ushort instance, XlsPicture picture) : base(version, instance) {
			this.picture = picture;
		}
		protected XlsPicture Picture { get { return picture; } }
		protected override int GetSize() {
			return picture.Size + XlsConsts.BLIP_Extra + MsoHeader.Size();
		}
		public override void Write(XlsStream stream) {
			fInstance = (ushort)blipTypes[picture.PictureType];
			uint length = Convert.ToUInt32(picture.Size + XlsConsts.BLIP_Extra);
			ushort fbt = Convert.ToUInt16(XlsConsts.MsoBLIP_Start + picture.PictureType);
			WriteMsoHeader(fbt, length, stream);
			byte[] buffer = new byte[XlsConsts.MaxRecSize97];
			buffer[16] = 0x004C;
			stream.Write(buffer, 0, XlsConsts.BLIP_Extra);
			byte[] pictBytes = picture.GetImageBytes();
			int written = 0;
			while(true) {
				byte[] content;
				int count = Math.Min(pictBytes.Length - written, XlsConsts.MaxRecSize97);
				if(count <= 0) break;
				content = new Byte[count];
				Array.Copy(pictBytes, written, content, 0, content.Length);
				stream.Write(content, 0, content.Length);
				written += content.Length;
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(picture != null)
					picture.Dispose();
			}
		}
	}
	
	public class MsoBseRec {
		public static int Size() {
			return 36; 
		}
		public XlsPictureType winType;
		public XlsPictureType macType;
		public byte[] checksum = new byte[16];
		public ushort tag;
		public uint size;
		public uint refCount;
		public uint delOffset;
		public byte isTexture;
		public byte nameLen;
		public ushort Dummy;
	}
	
	public class MsoDataBse : MsoDataBLIP {
		bool isDummy;
		public MsoDataBse(ushort version, ushort instance, XlsPicture picture, bool isDummy) : base(version, instance, picture) {
			this.isDummy = isDummy;
		}
		protected override int GetSize() {
			return MsoBseRec.Size() + MsoHeader.Size();
		}
		public override void Write(XlsStream stream) {
			MsoBseRec rec = new MsoBseRec();
			rec.tag = 0x00FF;
			if(!isDummy) {
				rec.winType = Picture.PictureType;
				rec.macType = rec.winType;
				rec.size = Convert.ToUInt32(Picture.Size + 25);
				rec.refCount = Convert.ToUInt32(Picture.RefCount);
				rec.Dummy = 0x02E2;
				fInstance = Convert.ToUInt16(rec.winType);
			}
			WriteMsoHeader(XlsConsts.MsoBse, Convert.ToUInt32(Picture.Size + XlsConsts.BLIP_Extra + DataSize + MsoHeader.Size()), stream);
			stream.Write(new Byte[1] { Convert.ToByte(rec.winType) } , 0, 1);
			stream.Write(new Byte[1] { Convert.ToByte(rec.macType) } , 0, 1);
			stream.Write(rec.checksum, 0, 16);
			stream.Write(rec.tag);
			stream.Write(rec.size);
			stream.Write(rec.refCount);
			stream.Write(rec.delOffset);
			stream.Write(rec.isTexture);
			stream.Write(rec.nameLen);
			stream.Write(rec.Dummy);
		}
	}
	
	public class ObjCmo {
		public static int Size() {
			return 3 * 2 + 12;
		}
		public ushort objType;
		public ushort objId;
		public ushort options;
		public byte[] reserved = new byte[12];
	}
	  
	public struct MsoSplitMenuColors {
		public static int Size() {
			return 4 * sizeof(uint);
		}
		public uint topLevelFill;
		public uint lineType;
		public uint shadow;
		public uint threeD;
	}
	
	public class MsoDataSplitMenuColors : MsoData {
		public MsoDataSplitMenuColors(ushort version, ushort instance) : base(version, instance) {
		}
		protected override int GetSize() {
			return MsoSplitMenuColors.Size() + MsoHeader.Size();
		}
		public override void Write(XlsStream stream) {
			MsoSplitMenuColors rec = new MsoSplitMenuColors();
			rec.topLevelFill = 0x0800000D;
			rec.lineType	 = 0x0800000C;
			rec.shadow	   = 0x08000017;
			rec.threeD	   = 0x100000F7;
			WriteMsoHeader(XlsConsts.MsoSplitMenuColors, DataSize, stream);
			stream.Write(rec.topLevelFill);
			stream.Write(rec.lineType);
			stream.Write(rec.shadow);
			stream.Write(rec.threeD);
		}
	}
	#region MsoDataContainer
	
	public class MsoDataContainer : MsoData {
		uint extSize = 0;
		ushort id;
		ArrayList items;
		public MsoDataContainer(ushort version, ushort instance, ushort id) : base(version, instance) {
			this.id = id;
			items = new ArrayList();
		}
		protected ArrayList Items { get { return items; } }
		public uint ExtSize { get { return extSize; } set { extSize = value; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				int count = Items.Count;
				for(int i = 0; i < count; i++) {
					((MsoData)Items[i]).Dispose();
				}
			}
		}
		public void AddItem(MsoData item) {
			Items.Add(item);
		}
		public override void Write(XlsStream stream) {
			uint size = Convert.ToUInt32(Size - MsoHeader.Size() + extSize);
			WriteMsoHeader(id, size, stream);
			int count = Items.Count;
			for(int i = 0; i < count; i++) {
				((MsoData)items[i]).Write(stream);
			}
		}
		protected override int GetSize() {
			int size = MsoHeader.Size();
			int count = Items.Count;
			for(int i = 0; i < count; i++) {
				size += ((MsoData)items[i]).Size;
			}
			return size;
		}
	}
	#endregion
	#region MsoWriteDrawing
	
	public class MsoWriteDrawing {
		SheetPictureCollection pictures;
		MsoDataContainer dgContainer;
		MsoDataContainer spgrContainer;
		ArrayList msoDataList;
		public MsoWriteDrawing(SheetPictureCollection pictures) {
			this.pictures = pictures;
			msoDataList = new ArrayList();
			CreateObjectHierarchy();
		}
		protected internal SheetPictureCollection Pictures { get { return pictures;	} }
		public void Dispoise() {
			dgContainer.Dispose();
			spgrContainer.Dispose();
		}
		void CreateObjectHierarchy() {
			this.dgContainer = new MsoDataContainer(0x0F, 0x00, XlsConsts.MsoDgContainer);
			this.spgrContainer = new MsoDataContainer(0x0F, 0x00, XlsConsts.MsoSpGrContainer);
			dgContainer.AddItem(new MsoDataDg(0x00, 0x01));
			dgContainer.AddItem(spgrContainer);
			MsoDataContainer spContainer = new MsoDataContainer(0x0F, 0x00, XlsConsts.MsoSpContainer);
			spContainer.AddItem(new MsoDataSpGr(0x01, 0x00));
			spContainer.AddItem(new MsoDataSp(0x02, 0x00, XlsConsts.MsoSpId - 1, 0x005));
			spgrContainer.AddItem(spContainer);
			int i = 0;
			foreach(SheetPicture pic in pictures) {
				spContainer = new MsoDataContainer(0x0F, 0x00, XlsConsts.MsoSpContainer);
				spContainer.AddItem(new MsoDataSp(0x02, XlsConsts.SpId_Pict, Convert.ToUInt32(XlsConsts.MsoSpId + i), 0x0A00));
				MsoDataOpt opt = new MsoDataOpt(0x03, 0x0C);
				opt.AddValue(0x4104, pic.ItemId) ; 
				opt.AddValue(0x01BF, 0x10000);
				opt.AddValue(0x03BF, 0x80008);
				spContainer.AddItem(opt);
				spContainer.AddItem(new MsoDataClientAnchor(0x00, 0x00, 0x0002, pic));
				spContainer.AddItem(new MsoDataClientData(0x00, 0x00));
				msoDataList.Add(spContainer);
				i++;
			}
		}
		public void WriteObject(int index, XlsStream stream) {
			if(index == 0) {
				dgContainer.ExtSize = GetMsoDataTotalSize();
				spgrContainer.ExtSize = dgContainer.ExtSize;
				int size = dgContainer.Size;
				if(msoDataList.Count > 0) 
					size += ((MsoData)msoDataList[0]).Size;
				stream.WriteHeader(XlsConsts.BIFFRecId_MsoDrawing, size);
				dgContainer.Write(stream);
				if(msoDataList.Count > 0) 
					((MsoData)msoDataList[0]).Write(stream);
			} else {
				MsoData item = (MsoData)msoDataList[index];
				stream.WriteHeader(XlsConsts.BIFFRecId_MsoDrawing, item.Size);
				item.Write(stream);
			}
		}
		protected uint GetMsoDataTotalSize() {
			uint size = 0;
			int count = msoDataList.Count;
			for(int i = 0; i < count; i++) {
				size += (uint)((MsoData)msoDataList[i]).Size;
			}
			return size;
		}
	}
	#endregion
	#region	SheetPictures
	
	public class SheetPicture: AhchorObject {
		const int maxHorizOffset = 1024;
		const int maxVertOffset = 256;
		public static SheetPicture CreateInstance(int col, int row, XlsPicture xlsPicture, float relativeHorizOffset, float relativeVertOffset) {
			SheetPicture pic = new SheetPicture(xlsPicture);
			pic.Col1 = Convert.ToUInt16(col);
			pic.Row1 = Convert.ToUInt16(row);
			pic.Col2 = pic.Col1;
			pic.Row2 = pic.Row1;
			pic.Col1Offset = Convert.ToUInt16(relativeHorizOffset * maxHorizOffset);
			pic.Row1Offset = Convert.ToUInt16(relativeVertOffset * maxVertOffset);
			return pic;
		}
		XlsPicture xlsPicture;
		public SheetPicture(XlsPicture xlsPicture) {
			this.xlsPicture = xlsPicture;
			ItemId = Convert.ToUInt16(xlsPicture.ItemId);
			Col2Offset = maxHorizOffset;
			Row2Offset = maxVertOffset;
		}
		public XlsPicture XlsPicture { get { return xlsPicture; } }
	}
	[ListBindable(BindableSupport.No)]
	public class SheetPictureCollection : IEnumerable {
		Hashtable picturesHT = new Hashtable();
		ArrayList pictures = new ArrayList();
		public int Count { get { return pictures.Count; } }
		IEnumerator IEnumerable.GetEnumerator() { return pictures.GetEnumerator(); }
		public void Add(SheetPicture obj) {
			if(!picturesHT.ContainsKey(obj)) {
				picturesHT.Add(obj, null);
				pictures.Add(obj);
			}
		}
	}
	#endregion
	#region	XlsPictures
	public enum XlsPictureType { 
		Jpeg = 5, Png = 6 
	};
	public class ImageHelper {
		private class HashObj {
			public static HashObj GetObj(Image image) {
				return new HashObj(image);
			}
			WeakReference weakRef;
			int imageHashCode;
			HashObj(Image image) {
				if(image == null)
					throw new ArgumentNullException("image");
				this.weakRef = new WeakReference(image, false);
				this.imageHashCode = image.GetHashCode();
			}
			public override int GetHashCode() {
				return imageHashCode;
			}
			public override bool Equals(object obj) {
				if(obj is HashObj) {
					HashObj hashObj = (HashObj)obj;
					return weakRef.Target == hashObj.weakRef.Target;
				}
				return false;
			}
		}
		static Hashtable imageHT = new Hashtable();
		public static byte[] GetImageBytes(Image image) {
			if(image == null)
				return new byte[] {};
			HashObj hashObj = HashObj.GetObj(image);
			byte[] bytes = (byte[])imageHT[hashObj];
			if(bytes == null) {
				bytes = ByteImageConverter.ToByteArray(image, ImageFormat.Png);
				imageHT[hashObj] = bytes;
			}
			return bytes;
		}
		public static void Reset() {
			imageHT.Clear();
		}
	}
	public class XlsPicture : IDisposable {
		Image image;
		Image innerImage;
		XlsPictureType pictureType = XlsPictureType.Png;
		byte[] bytes;
		bool isDisposed;
		int size = 0;
		int refCount = 0;
		int itemId = -1;
		public XlsPicture(Image image) {
			if(image == null)
				throw new ArgumentException("null is not a valid value for image");
			this.image = image;
			if(!SupportedImageFormat(image.RawFormat))
				this.innerImage = ConvertToPng(image);
			UpdatePictureType();
			bytes = ImageHelper.GetImageBytes(GetActualImage());
			this.size = bytes.Length;
		}
		public Image Image { get { return image; } }
		public XlsPictureType PictureType { get { return pictureType; } }
		public int Size { get { return size;} }
		protected internal int RefCount { get { return refCount; } set { refCount = value;} } 
		protected internal int ItemId { get { return itemId; } set { itemId = value; } }
		protected internal Image InnerImage { get { return innerImage; } }
		protected internal bool IsDisposed { get { return isDisposed; } }
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				image = null;
				if(innerImage != null) {
					innerImage.Dispose();
					innerImage = null;
				}
			}
			this.isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~XlsPicture() {
			Dispose(false);
		}
		#endregion
		public byte[] GetImageBytes() {
			return bytes;
		}
		protected internal Image GetActualImage() {
			return innerImage != null ? innerImage : image;
		}
		void UpdatePictureType() {
			this.pictureType = (image.RawFormat == ImageFormat.Jpeg) ? XlsPictureType.Jpeg : XlsPictureType.Png;
		}
		bool SupportedImageFormat(ImageFormat format) { 
			return format.Equals(ImageFormat.Jpeg) || format.Equals(ImageFormat.Png);
		}
		internal static Image ConvertToPng(Image image) {
			Image png;
			MemoryStream stream = new MemoryStream();
			try {
				ImageCodecInfo info = FindEncoder(ImageFormat.Png);
				image.Save(stream, info, null);
				png = Image.FromStream(stream);
			} finally {
				stream.Close();
			}
			return png;
		}
		static ImageCodecInfo FindEncoder(ImageFormat format) {
			ImageCodecInfo[] infos = ImageCodecInfo.GetImageEncoders();
			for (int i = 0; i < infos.Length; i++) {
				if(infos[i].FormatID.Equals(format.Guid)) {
					return infos[i];
				}
			}
			return null;
		}
	}
	[ListBindable(BindableSupport.No)]
	public class XlsPictureCollection: CollectionBase {
		Dictionary<Image, XlsPicture> imageHash;
		public XlsPictureCollection() {
			imageHash = new Dictionary<Image, XlsPicture>();
		}
		public XlsPicture this[int index] { get { return (XlsPicture)List[index]; }	}
		protected internal Dictionary<Image, XlsPicture> ImageHash { get { return imageHash; } }
		public int Add(XlsPicture obj) {
			return List.Contains(obj) ? List.IndexOf(obj) : List.Add(obj);
		}
		protected override void OnClear() {
			base.OnClear();
			for(int i = 0; i < Count; i++) {
				this[i].Dispose();
			}
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			XlsPicture pic = (XlsPicture)value;
			pic.ItemId = index + 1;
			ImageHash.Add(pic.Image, pic);
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			XlsPicture pic = (XlsPicture)value;
			ImageHash.Remove(pic.Image);
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			ImageHash.Clear();
		}
		public XlsPicture GetByImage(Image image) {
			if(image == null) return null;
			XlsPicture pic;
			if(ImageHash.TryGetValue(image, out pic))
				return pic;
			byte[] data = ByteImageConverter.ToByteArray(image, ImageFormat.Png);
			foreach(XlsPicture picture in this) {
				byte[] bytes = picture.GetImageBytes();
				if(bytes.Length == data.Length) {
					int i;
					for(i = 0; i < data.Length; i++)
						if(bytes[i] != data[i])
							break;
					if(i == data.Length) {
						return picture;
					}
				}
			}
			return AddNew(image);
		}
		protected XlsPicture AddNew(Image image) {
			XlsPicture pic = new XlsPicture(image);
			Add(pic);
			return pic;
		}
	}
	#endregion
	#region XlsPictureWriter
	
	public class XlsPictureWriter : IDisposable {
		MsoDataContainer dggContainer;
		MsoWriteDrawing msoDrawing;
		public XlsPictureWriter() {
		}
		protected MsoDataContainer DggContainer { get {	return dggContainer; } }
		protected MsoWriteDrawing MsoDrawing { get { return msoDrawing; } }
		public bool IsReady { get { return DggContainer != null && MsoDrawing != null; } }
		public void Dispose() {
			if(dggContainer != null) 
				dggContainer.Dispose();
			if(msoDrawing != null) 
				msoDrawing.Dispoise();
		}
		public void CreateObjectHierarchy(XlsPictureCollection xlsPictures, SheetPictureCollection sheetPictures) {
			if(xlsPictures.Count == 0 || sheetPictures.Count == 0)
				return;
			CreateMsoDrawingGroup(xlsPictures);
			CreateMsoDrawing(sheetPictures);
		}
		public void WriteMsoDrawingGroup(XlsStream stream) {
			if(!IsReady) return;
			stream.BeginContinueWrite();
			stream.WriteHeader(XlsConsts.BIFFRecId_MsoDrawingGroup, dggContainer.Size);
			dggContainer.Write(stream);
			stream.EndContinueWrite();
		}
		public void WriteMsoDrawing(XlsStream stream) {
			if(!IsReady) return;
			int i = 0;
			foreach(SheetPicture pic in msoDrawing.Pictures) {
				msoDrawing.WriteObject(i, stream);
				stream.WriteHeader(XlsConsts.BIFFRecId_Obj, ObjCmo.Size() + 4 * BIFFHeader.Size());
				stream.WriteHeader(XlsConsts.ObjRec_Cmo, ObjCmo.Size());
				ObjCmo rec = new ObjCmo();
				rec.objType = 0x08;
				rec.objId = pic.ItemId;
				rec.options = 0x0011;
				stream.Write(rec.objType);
				stream.Write(rec.objId);
				stream.Write(rec.options);
				stream.Write(rec.reserved, 0, 12);
				stream.WriteHeader(XlsConsts.ObjRec_CF, 0);
				stream.WriteHeader(XlsConsts.ObjRec_PioGrbit, 0);
				stream.WriteHeader(XlsConsts.ObjRec_End, 0);
				i++;
			}
		}
		void CreateMsoDrawingGroup(XlsPictureCollection pictures) {
			if(pictures.Count == 0)
				return;
			this.dggContainer = new MsoDataContainer(0x0F, 0x00, XlsConsts.MsoDggContainer);
			MsoDataOpt opt = new MsoDataOpt(0x03, 0x03);
			dggContainer.AddItem(new MsoDataDgg(0x0, 0x0, pictures.Count));
			if(pictures.Count > 0) {
				MsoDataContainer bStoreContainer = new MsoDataContainer(0x0F, 0x01, XlsConsts.MsoBStoreContainer);
				for(int i = 0; i < pictures.Count; i++) {
					bStoreContainer.AddItem(new MsoDataBse(0x02, 0x05, pictures[i], false));
					bStoreContainer.AddItem(new MsoDataBLIP(0x00, 0x46A, pictures[i]));
				}
				dggContainer.AddItem(bStoreContainer);
			}
			opt.AddValue(0x00BF,0x00080008);
			opt.AddValue(0x0181,0x08000009);
			opt.AddValue(0x01C0,0x08000040);
			dggContainer.AddItem(opt);
			dggContainer.AddItem(new MsoDataSplitMenuColors(0x00, 0x04));
		}
		void CreateMsoDrawing(SheetPictureCollection pictures) {
			this.msoDrawing = new MsoWriteDrawing(pictures);
		}
	}
	#endregion
	#region ByteImageConverter
	public class ByteImageConverter {
		protected static Image FromByteArray(byte[] b, int offset) {
			if(b == null)
				return null;
			Image tempI = null;
			System.IO.MemoryStream s = new System.IO.MemoryStream(b, offset, (int)b.Length - offset);
			try {
				tempI = Image.FromStream(s);
			}
			catch {}
			return tempI;
		}
		public static Image FromByteArray(byte[] b) {
			if(b == null)
				return null;
			Image i = null;
			if(b.Length > 78) {
				if(b[0] == 0x15 && b[1] == 0x1c)  
					i = FromByteArray(b, 78);
			}
			if(i == null)
				i = FromByteArray(b, 0);
			return i;
		}
		static ImageCodecInfo FindEncoder(ImageFormat format) {
			ImageCodecInfo[] list = ImageCodecInfo.GetImageEncoders();
			foreach(ImageCodecInfo ici in list) {
				if(ici.FormatID.Equals(format.Guid)) {
					return ici;
				}
			}
			return null;
		}
		public static byte[] ToByteArray(Image image, ImageFormat imageFormat) {
			if(image == null)
				return null;
			System.IO.MemoryStream s = new System.IO.MemoryStream();
			try {
				ImageCodecInfo ici = FindEncoder(image.RawFormat);
				if(ici != null)
					image.Save(s, ici, null);
				else
					image.Save(s, imageFormat);
			}
			catch {
				return null;
			}
			byte[] ret =  s.ToArray();
			s.Close();
			return ret;
		}
		public static byte[] ToByteArray(object obj) {
			byte[] arr;
			if(!(obj is byte[])) return null;
			try {
				arr = (byte[])obj;
			}
			catch {
				arr = null;
			}
			return arr;
		}
	}
	#endregion
	#endregion
}
