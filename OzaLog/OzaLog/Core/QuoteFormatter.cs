using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace OzaLog.Core
{
    /// <summary>
    /// 報價格式化器 - 把 <see cref="QuoteRecord"/> 渲染成單行字串(txt/log/json 三種格式)。
    /// </summary>
    /// <remarks>
    /// v3.1+ 引入,獨立於主 logger 的 LogFormatter / JsonLogFormatter。
    ///
    /// 格式對照(來自需求 3 拍板):
    /// • Txt/Log:<c>[2026-05-13 10:23:45.123] binance_spot BTCUSDT last=60123.5 bid=60123.0 ask=60124.0</c>
    ///   - ISO 8601 風格時間(給人看)
    ///   - 只輸出非 null 欄位(空欄位略過)
    ///   - Extras 展開成更多 k=v
    /// • Json:NDJSON,epoch_ms,只輸出非 null,Extras nested 在 <c>extras</c> 子物件
    ///   - 不含 thread 資訊(報價是市場事件,非程式內部事件)
    /// </remarks>
    internal static class QuoteFormatter
    {
        private const long UnixEpochTicks = 621355968000000000L;

        // 內建欄位名(用於撞名檢查 + JSON / 文字輸出 key 名稱統一)
        private static readonly HashSet<string> _reservedKeys = new HashSet<string>(StringComparer.Ordinal)
        {
            "ts", "symbol", "bucket", "last", "lastQty",
            "bid", "bidQty", "ask", "askQty",
            "open", "prevClose", "high", "low",
            "volume", "quoteVolume", "extras",
        };

        private static readonly JsonWriterOptions _writerOptions = new JsonWriterOptions
        {
            Indented = false,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        /// <summary>
        /// 依設定格式渲染單筆 QuoteRecord 為單行字串(不含換行符)
        /// </summary>
        public static string Format(in QuoteRecord rec, QuoteOutputFormat format)
        {
            ValidateExtras(in rec);

            switch (format)
            {
                case QuoteOutputFormat.Json:
                    return FormatJson(in rec);
                case QuoteOutputFormat.Log:
                case QuoteOutputFormat.Txt:
                default:
                    return FormatText(in rec);
            }
        }

        /// <summary>
        /// 驗證 Extras / ExtrasJson 是否衝突或撞名內建欄位
        /// </summary>
        private static void ValidateExtras(in QuoteRecord rec)
        {
            if (rec.Extras != null && !string.IsNullOrEmpty(rec.ExtrasJson))
            {
                throw new ArgumentException(
                    "QuoteRecord.Extras 與 QuoteRecord.ExtrasJson 不可同時設定(請擇一)",
                    nameof(QuoteRecord.Extras));
            }

            if (rec.Extras != null)
            {
                foreach (var kv in rec.Extras)
                {
                    if (_reservedKeys.Contains(kv.Key))
                    {
                        throw new ArgumentException(
                            $"QuoteRecord.Extras key '{kv.Key}' 撞名內建欄位,請改用其他名稱",
                            nameof(QuoteRecord.Extras));
                    }
                }
            }
        }

        // =========================================================
        // Txt / Log:[ISO time] bucket symbol k=v k=v ...
        // =========================================================
        private static string FormatText(in QuoteRecord rec)
        {
            var sb = new StringBuilder(160);
            var dt = new DateTime(rec.Ticks, DateTimeKind.Local);

            sb.Append('[')
              .Append(dt.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture))
              .Append("] ");

            sb.Append(rec.Bucket).Append(' ').Append(rec.Symbol);

            sb.Append(" last=").Append(rec.Last.ToString(CultureInfo.InvariantCulture));

            AppendOptional(sb, "lastQty", rec.LastQty);
            AppendOptional(sb, "bid", rec.Bid);
            AppendOptional(sb, "bidQty", rec.BidQty);
            AppendOptional(sb, "ask", rec.Ask);
            AppendOptional(sb, "askQty", rec.AskQty);
            AppendOptional(sb, "open", rec.Open);
            AppendOptional(sb, "prevClose", rec.PrevClose);
            AppendOptional(sb, "high", rec.High);
            AppendOptional(sb, "low", rec.Low);
            AppendOptional(sb, "volume", rec.Volume);
            AppendOptional(sb, "quoteVolume", rec.QuoteVolume);

            // Extras:dictionary → k=v 展開
            if (rec.Extras != null)
            {
                foreach (var kv in rec.Extras)
                {
                    sb.Append(' ').Append(kv.Key).Append('=')
                      .Append(ValueToText(kv.Value));
                }
            }

            // ExtrasJson:嘗試 parse 並展開
            if (!string.IsNullOrEmpty(rec.ExtrasJson))
            {
                AppendExtrasJsonAsText(sb, rec.ExtrasJson);
            }

            return sb.ToString();
        }

        private static void AppendOptional(StringBuilder sb, string key, decimal? value)
        {
            if (!value.HasValue) return;
            sb.Append(' ').Append(key).Append('=')
              .Append(value.Value.ToString(CultureInfo.InvariantCulture));
        }

        private static string ValueToText(object value)
        {
            if (value == null) return string.Empty;
            if (value is IFormattable f) return f.ToString(null, CultureInfo.InvariantCulture);
            return value.ToString();
        }

        private static void AppendExtrasJsonAsText(StringBuilder sb, string extrasJson)
        {
            // 嘗試 parse 並展開 top-level properties 為 k=v;失敗則丟原字串到 extrasJson=
            try
            {
                using var doc = JsonDocument.Parse(extrasJson);
                if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in doc.RootElement.EnumerateObject())
                    {
                        if (_reservedKeys.Contains(prop.Name))
                        {
                            throw new ArgumentException(
                                $"QuoteRecord.ExtrasJson 的 key '{prop.Name}' 撞名內建欄位,請改用其他名稱",
                                nameof(QuoteRecord.ExtrasJson));
                        }
                        sb.Append(' ').Append(prop.Name).Append('=').Append(prop.Value.ToString());
                    }
                    return;
                }
            }
            catch (JsonException)
            {
                // fallthrough — 不是合法 JSON,當作 opaque 字串
            }
            sb.Append(" extrasJson=").Append(extrasJson);
        }

        // =========================================================
        // Json:NDJSON,epoch_ms,只輸出非 null,Extras nested 在 extras
        // =========================================================
        private static string FormatJson(in QuoteRecord rec)
        {
            using var stream = new MemoryStream(256);
            using (var writer = new Utf8JsonWriter(stream, _writerOptions))
            {
                writer.WriteStartObject();

                var utcTicks = new DateTime(rec.Ticks, DateTimeKind.Local).ToUniversalTime().Ticks;
                var epochMs = (utcTicks - UnixEpochTicks) / TimeSpan.TicksPerMillisecond;
                writer.WriteNumber("ts", epochMs);

                writer.WriteString("symbol", rec.Symbol);
                writer.WriteString("bucket", rec.Bucket);
                writer.WriteNumber("last", rec.Last);

                WriteOptional(writer, "lastQty", rec.LastQty);
                WriteOptional(writer, "bid", rec.Bid);
                WriteOptional(writer, "bidQty", rec.BidQty);
                WriteOptional(writer, "ask", rec.Ask);
                WriteOptional(writer, "askQty", rec.AskQty);
                WriteOptional(writer, "open", rec.Open);
                WriteOptional(writer, "prevClose", rec.PrevClose);
                WriteOptional(writer, "high", rec.High);
                WriteOptional(writer, "low", rec.Low);
                WriteOptional(writer, "volume", rec.Volume);
                WriteOptional(writer, "quoteVolume", rec.QuoteVolume);

                // Extras nested 在 extras 子物件(使用者拍板:用 nested 而非 top-level 合併)
                if (rec.Extras != null && rec.Extras.Count > 0)
                {
                    writer.WriteStartObject("extras");
                    foreach (var kv in rec.Extras)
                    {
                        WriteExtraValue(writer, kv.Key, kv.Value);
                    }
                    writer.WriteEndObject();
                }
                else if (!string.IsNullOrEmpty(rec.ExtrasJson))
                {
                    WriteExtrasJsonAsObject(writer, rec.ExtrasJson);
                }

                writer.WriteEndObject();
            }

            return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
        }

        private static void WriteOptional(Utf8JsonWriter writer, string key, decimal? value)
        {
            if (!value.HasValue) return;
            writer.WriteNumber(key, value.Value);
        }

        private static void WriteExtraValue(Utf8JsonWriter writer, string key, object value)
        {
            if (value == null) { writer.WriteNull(key); return; }

            switch (value)
            {
                case string s: writer.WriteString(key, s); break;
                case bool b: writer.WriteBoolean(key, b); break;
                case decimal dec: writer.WriteNumber(key, dec); break;
                case double d: writer.WriteNumber(key, d); break;
                case float f: writer.WriteNumber(key, f); break;
                case long l: writer.WriteNumber(key, l); break;
                case int i: writer.WriteNumber(key, i); break;
                case short sh: writer.WriteNumber(key, sh); break;
                case byte bt: writer.WriteNumber(key, bt); break;
                case ulong ul: writer.WriteNumber(key, ul); break;
                case uint ui: writer.WriteNumber(key, ui); break;
                case ushort us: writer.WriteNumber(key, us); break;
                case sbyte sb: writer.WriteNumber(key, sb); break;
                case DateTime dt: writer.WriteString(key, dt.ToString("O", CultureInfo.InvariantCulture)); break;
                case DateTimeOffset dto: writer.WriteString(key, dto.ToString("O", CultureInfo.InvariantCulture)); break;
                default: writer.WriteString(key, value.ToString()); break;
            }
        }

        private static void WriteExtrasJsonAsObject(Utf8JsonWriter writer, string extrasJson)
        {
            try
            {
                using var doc = JsonDocument.Parse(extrasJson);
                if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    writer.WritePropertyName("extras");
                    // 撞名檢查:逐 property 確認
                    foreach (var prop in doc.RootElement.EnumerateObject())
                    {
                        if (_reservedKeys.Contains(prop.Name))
                        {
                            throw new ArgumentException(
                                $"QuoteRecord.ExtrasJson 的 key '{prop.Name}' 撞名內建欄位,請改用其他名稱",
                                nameof(QuoteRecord.ExtrasJson));
                        }
                    }
                    doc.RootElement.WriteTo(writer);
                    return;
                }
            }
            catch (JsonException)
            {
                // 不是合法 JSON object,放入 extras 作為單一 _raw 欄位
            }
            writer.WriteStartObject("extras");
            writer.WriteString("_raw", extrasJson);
            writer.WriteEndObject();
        }
    }
}
