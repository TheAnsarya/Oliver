using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// Readonly stream made up of multiple files based on a List of FileInfo objects
// Only one file stream is open at a time
// TODO: Check for nulls and not exists in files list?
// TODO: Handle exceptions like file doesn't exist here or just let them bubble up?
namespace Oliver.Domain.Streams {
	public class MuliFileStream : Stream {
		private ReadOnlyCollection<FileInfo> Files { get; }

		private Stream _currentStream;

		private int CurrentFileIndex { get; set; }

		public bool Done { get; set; }

		public override bool CanRead => true;

		public override bool CanSeek => false;

		public override bool CanWrite => false;

		private long _length = -1;
		public override long Length {
			get {
				if (_length == -1) {
					_length = Files.Sum(x => x.Length);
				}

				return _length;
			}
		}

		private long _position;
		public override long Position {
			get => _position;
			set => throw new NotImplementedException();
		}

		public MuliFileStream(List<FileInfo> files) : this(files?.AsReadOnly()) { }

		public MuliFileStream(ReadOnlyCollection<FileInfo> files) =>
			Files = files ?? throw new ArgumentNullException(nameof(files));

		public override void Flush() => _currentStream?.Flush();

		public override int Read(byte[] buffer, int offset, int count) {
			// Short circuit when nothing left to read
			if (Done) {
				return 0;
			}

			// Ensure there is a current stream, should usually only be on first read
			// Handles the case when Files is empty
			if (_currentStream == null) {
				if (CurrentFileIndex >= Files.Count) {
					Done = true;
					return 0;
				}

				_currentStream = Files[CurrentFileIndex].OpenRead();
			}

			var result = 0;
			var buffPostion = offset;

			while (count > 0) {
				var bytesRead = _currentStream.Read(buffer, buffPostion, count);
				result += bytesRead;
				buffPostion += bytesRead;
				_position += bytesRead;
				count -= bytesRead;

				if (count > 0) {
					_currentStream.Dispose();
					CurrentFileIndex++;

					if (CurrentFileIndex >= Files.Count) {
						Done = true;
						_currentStream = null;
						break;
					}

					_currentStream = Files[CurrentFileIndex].OpenRead();
				}
			}

			return result;
		}
		public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
			// Short circuit when nothing left to read
			if (Done) {
				return 0;
			}

			// Ensure there is a current stream, should usually only be on first read
			// Handles the case when Files is empty
			if (_currentStream == null) {
				if (CurrentFileIndex >= Files.Count) {
					Done = true;
					return 0;
				}

				_currentStream = Files[CurrentFileIndex].OpenRead();
			}

			var result = 0;
			var buffPostion = offset;

			while (count > 0) {
				var bytesRead = await _currentStream.ReadAsync(buffer.AsMemory(buffPostion, count), cancellationToken);
				result += bytesRead;
				buffPostion += bytesRead;
				_position += bytesRead;
				count -= bytesRead;

				if (count > 0) {
					_currentStream.Dispose();
					CurrentFileIndex++;

					if (CurrentFileIndex >= Files.Count) {
						Done = true;
						_currentStream = null;
						break;
					}

					_currentStream = Files[CurrentFileIndex].OpenRead();
				}
			}

			return result;
		}

		public override long Seek(long offset, SeekOrigin origin) =>
			throw new InvalidOperationException("Stream is not seekable");

		// TODO: do we need to implement this?
		public override void SetLength(long value) => throw new NotImplementedException();

		public override void Write(byte[] buffer, int offset, int count) =>
			throw new InvalidOperationException("Stream is not writable");

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
			throw new InvalidOperationException("Stream is not writable");

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			_currentStream?.Dispose();
			_currentStream = null;
		}
	}
}
