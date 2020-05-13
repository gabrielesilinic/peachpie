﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Peachpie.NET.Sdk
{
    /// <summary>
    /// Helper methods for resolving SPDX license expressions.
    /// </summary>
    public static class SpdxHelpers
    {
        static readonly Dictionary<string, string> s_spdx_fixes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "Apache 2.0", "Apache-2.0" },
            { "Apache License", "Apache-2.0" },
            { "Apache License 2", "Apache-2.0" },
            { "BSD", "BSD-2-Clause" },
            { "BSD 2", "BSD-2-Clause" },
            { "BSD License", "BSD-2-Clause" },
            { "GPL 2.0", "GPL-2.0-or-later" },
            // deprecations:
            { "GPL-2.0", "GPL-2.0-or-later" },
            { "GPL-2.0+", "GPL-2.0-or-later" },
            { "AGPL-3.0", "AGPL-3.0-or-later" },
            { "AGPL-3.0+", "AGPL-3.0-or-later" },
            { "LGPL-2.0+", "LGPL-2.0-or-later" },
            { "LGPL-2.1", "LGPL-2.1-or-later" },
            { "LGPL-2.1+", "LGPL-2.1-or-later" },
            { "LGPL-3.0", "LGPL-3.0-or-later" },
            { "LGPL-3.0+", "LGPL-3.0-or-later" },
            // operators:
            { "or", "OR" },
        };

        static readonly char[] s_spdx_separators = new[] { ' ', '(', ')', ';', };

        /// <summary>
        /// Handles common typos and invalid license strings,
        /// translates it to valid SPDX recognized by NuGet task.
        /// </summary>
        public static string SanitizeSpdx(string spdx)
        {
            if (string.IsNullOrEmpty(spdx))
            {
                return string.Empty;
            }

            // there are commonly used deprecations and invalid expressions,
            // fix the well-known issues:

            if (s_spdx_fixes.TryGetValue(spdx, out var newspdx))
            {
                spdx = newspdx;
            }
            else if (spdx.IndexOfAny(s_spdx_separators) != -1) // might be an expression
            {
                // naively get tokens between known separators and replace known deprecations
                int index = 0;
                while (index < spdx.Length)
                {
                    int sep = spdx.IndexOfAny(s_spdx_separators, index);
                    if (sep < 0)
                    {
                        // end of string
                        sep = spdx.Length;
                    }

                    //
                    var length = sep - index; // word length
                    if (length > 1)
                    {
                        var span = spdx.AsSpan(index);

                        //
                        foreach (var pair in s_spdx_fixes.OrderByDescending(p => p.Key.Length))
                        {
                            if (pair.Key.Length >= length && span.StartsWith(pair.Key.AsSpan(), StringComparison.OrdinalIgnoreCase))
                            {
                                // replace matched pair.Key with pair.Value:
                                spdx = spdx.Remove(index) + pair.Value + spdx.Substring(index + pair.Key.Length);
                                sep = index + pair.Value.Length;
                                break;
                            }
                        }
                    }

                    //
                    index = sep + 1;
                }
            }

            //
            return spdx;
        }
    }
}
