﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Shared.Extensions;

namespace Microsoft.CodeAnalysis.ChangeSignature
{
    internal sealed class SignatureChange
    {
        public readonly ParameterConfiguration OriginalConfiguration;
        public readonly ParameterConfiguration UpdatedConfiguration;

        private readonly Dictionary<int, int?> _originalIndexToUpdatedIndexMap = new Dictionary<int, int?>();

        public SignatureChange(ParameterConfiguration originalConfiguration, ParameterConfiguration updatedConfiguration)
        {
            OriginalConfiguration = originalConfiguration;
            UpdatedConfiguration = updatedConfiguration;

            // TODO: Could be better than O(n^2)
            var originalParameterList = originalConfiguration.ToListOfParameters();
            var updatedParameterList = updatedConfiguration.ToListOfParameters();

            for (var i = 0; i < originalParameterList.Count; i++)
            {
                var parameter = originalParameterList[i];
                var updatedIndex = updatedParameterList.IndexOf(p => p.Symbol == parameter.Symbol);
                _originalIndexToUpdatedIndexMap.Add(i, updatedIndex != -1 ? updatedIndex : (int?)null);
            }
        }

        public int? GetUpdatedIndex(int parameterIndex)
        {
            if (parameterIndex >= OriginalConfiguration.ToListOfParameters().Count)
            {
                return null;
            }

            return _originalIndexToUpdatedIndexMap[parameterIndex];
        }

        internal SignatureChange WithoutAddedParameters()
            => new SignatureChange(OriginalConfiguration, UpdatedConfiguration.WithoutAddedParameters());
    }
}
