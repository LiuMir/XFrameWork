using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public static class ZipUtility
{
	struct ZipInfo													//压缩数据结构
	{
		public string src;											//zip路径
		public string dst;											//解压路径
	}
	const int				LEVEL = 9;								//压缩等级(等级越高，压缩文件尺寸越小，耗时越多)
	static Thread			m_thread;								//解压线程
	static long				m_size;									//解压后文件总大小
	static long				m_sum;									//已解压文件大小
	static float			m_progress = 0;							//当前解压进度(0-1)
	static bool				m_isRunning = false;					//当前有文件正在解压
	static Queue<ZipInfo>	m_waitingList = new Queue<ZipInfo>();	//待解压列表
	public static float progress { get { return m_progress; } }		//当前解压进度(0-1)
	public static bool isRunning { get { return m_isRunning; } }	//当前有文件正在解压

	static ZipUtility()
	{
		ZipConstants.DefaultCodePage = 0;
	}

	/// <summary>
	/// 压缩单个文件/文件夹，默认压缩至当前文件夹
	/// </summary>
	/// <param name="src">源文件</param>
	/// <param name="dst">目标位置</param>
	public static void Compress(string src, string dst = null)
	{
		if(new FileInfo(src).Attributes == FileAttributes.Directory)
		{
			if(string.IsNullOrEmpty(dst))
			{
				dst = src + ".zip";
			}
			FastZip zip = new FastZip();
			zip.CreateEmptyDirectories = true;
			zip.CreateZip(dst, src, true, null);
		}
		else
		{
			if (string.IsNullOrEmpty(dst))
			{
				dst = Path.ChangeExtension(src, ".zip");
			}
			using (ZipFile zip = ZipFile.Create(dst))
			{
				zip.BeginUpdate();
				zip.Add(src, Path.GetFileName(src));
				zip.CommitUpdate();
			}
		}
	}

	/// <summary>
	/// 解压缩，默认解压至当前文件夹
	/// </summary>
	/// <param name="src">压缩文件</param>
	/// <param name="dst">解压路径</param>
	public static void Decompress(string src, string dst = null, bool overwrite = true)
	{
		if (string.IsNullOrEmpty(dst))
		{
			dst = Path.GetDirectoryName(src);
		}

		using (ZipInputStream zipStream = new ZipInputStream(File.OpenRead(src)))
		{
			for(ZipEntry entry = zipStream.GetNextEntry(); entry != null; entry = zipStream.GetNextEntry())
			{
				string path = Path.Combine(dst, entry.Name);
				// 创建目录
				if (entry.IsDirectory)
				{
					if (!Directory.Exists(path))
					{
						Directory.CreateDirectory(path);
					}
				}
				else
				{

					if (File.Exists(path))
					{
						if(overwrite)
						{
							//如果需要覆盖的话先删除旧文件
							File.Delete(path);
						}
						else
						{
							//文件已存在
							if (m_size != 0)
							{
								m_sum += entry.Size;
								m_progress = (float)m_sum / m_size;
							}
							continue;
						}
					}
					string tmpPath = path + ".tmp";
					using (FileStream streamWriter = File.Open(tmpPath, FileMode.OpenOrCreate, FileAccess.Write))
					{
						int size = 0;
						byte[] data = new byte[4096];
						while( (size = zipStream.Read(data, 0, data.Length)) != 0)
						{
							streamWriter.Write(data, 0, size);
							if (m_size != 0)
							{
								m_sum += size;
								m_progress = (float)m_sum / m_size;
							}
						}
					}
					//重命名
					File.Move(tmpPath, path);
				}
			}
		}
		File.Delete(src);
	}

	/// <summary>
	/// 多线程解压
	/// </summary>
	/// <param name="src"></param>
	/// <param name="dst"></param>
	public static void DecompressInBackgroud(string src, string dst = null)
	{
		m_waitingList.Enqueue(new ZipInfo() { src = src, dst = dst});
		if(!m_isRunning)
		{
			m_isRunning = true;
			m_thread = new Thread(DecompressThread);
			m_thread.IsBackground = true;
			m_thread.Start();
		}
	}

	/// <summary>
	/// 解压线程
	/// </summary>
	static void DecompressThread()
	{
		while(m_waitingList.Count > 0)
		{
			string src;
			string dst;
			lock (m_waitingList)
			{
				ZipInfo zipInfo = m_waitingList.Dequeue();
				src = zipInfo.src;
				dst = zipInfo.dst;
			}
			m_size		= 0;
			m_progress	= 0;
			m_sum		= 0;
			//计算zip解压后的大小
			using (ZipFile zip = new ZipFile(src))
			{
				foreach (ZipEntry ze in zip)
				{
					m_size += ze.Size;
				}
			}
			Decompress(src, dst, false);
			m_progress = 1;
		}
		m_isRunning = false;
		m_thread.Abort();
	}

	public static void Dispose()
	{
		if(m_thread != null)
		{
			m_thread.Abort();
			m_thread = null;
		}
	}

}

