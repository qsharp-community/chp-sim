// Copyright (c) Sarah Kaiser. All rights reserved.
// Licensed under the MIT License.
// Adapted from State display encoders in the IQSharp project here: 
// https://github.com/microsoft/iqsharp/blob/master/src/Jupyter/Visualization/StateDisplayEncoders.cs
#nullable enable

using System;
using Microsoft.Jupyter.Core;

namespace QSharpCommunity.Simulators.Chp
{
    public class StabilizerTableau
    {
        public bool[,] Data { get; set; }
    }

    public class TableauToTextEncoder : IResultEncoder
    {
        public string MimeType => MimeTypes.PlainText;

        public EncodedData? Encode(object displayable)
        {
            if (displayable is StabilizerTableau tableau)
            {
                return tableau.Data.MatrixToString(true).ToEncodedData();
            }
            else return null;
        }
    }

    public class TableauToHtmlEncoder : IResultEncoder
    {
        public string MimeType => MimeTypes.Html;

        public EncodedData? Encode(object displayable)
        {
            if (displayable is StabilizerTableau tableau)
            {
                var nQubits = tableau.Data.GetLength(0)/2;
                var tableFormat = new String('c', nQubits);
                //Print out table headers/labels
                // var headerRow = tableau.Data.$@"
                        // <tr>
                            // <th span=""{nQubits}"">$x$ on $i^{{th}}$ qubit</th>
                            // <th span=""{nQubits}"">$z$ on $i^{{th}}$ qubit</th>
                            // <th>phase</th>
                        // </tr>
                    // ";
                // Making rows
                //Putting it together
                // var outputTable = $@"
                    // <table style=""table-layout: fixed; width: 100%"">
                        // <thead>
                            // {headerRow}
                        // </thead>
                        // <tbody>
                            // {formattedData}
                        // </tbody>
                    // </table>
                // ";
                var tableRows = tableau.Data.MatrixToLatexString(true);

                var outputTable = $@"
                $\left(\begin{{array}}{{{tableFormat}|{tableFormat}|c}}
                {tableRows}
                \end{{array}} \right)$";

                return outputTable.ToEncodedData();
            }
            else return null;
        }
    }
}
