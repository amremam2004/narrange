#region Header

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * Copyright (c) 2007-2008 James Nies and NArrange contributors.
 *    All rights reserved.
 *
 * This program and the accompanying materials are made available under
 * the terms of the Common Public License v1.0 which accompanies this
 * distribution.
 *
 * Redistribution and use in source and binary forms, with or
 * without modification, are permitted provided that the following
 * conditions are met:
 *
 * Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright
 * notice, this list of conditions and the following disclaimer in
 * the documentation and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
 * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY
 * OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * Contributors:
 *      James Nies
 *      - Initial creation
 *      - Added special processing for arranging condition directive
 *        elements
 *      Justin Dearing
 *      - Code cleanup via ReSharper 4.0 (http://www.jetbrains.com/resharper/)
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

namespace NArrange.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using NArrange.Core.CodeElements;
    using NArrange.Core.Configuration;

    /// <summary>
    /// Code arranger.
    /// </summary>
    public sealed class CodeArranger : ICodeArranger
    {
        #region Fields

        /// <summary>
        /// Arranger chain collection synch lock.
        /// </summary>
        private readonly object _codeArrangeChainLock = new object();

        /// <summary>
        /// Code configuration.
        /// </summary>
        private readonly CodeConfiguration _configuration;

        /// <summary>
        /// Arranger chain collection.
        /// </summary>
        private ChainElementArranger _elementArrangerChain;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new code arranger with the specified configuration.
        /// </summary>
        /// <param name="configuration">Configuration to use for arranging code members.</param>
        public CodeArranger(CodeConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            // Clone the configuration information so we don't have to worry about it
            // changing during processing.
            _configuration = configuration.Clone() as CodeConfiguration;
        }

        #endregion Constructors

        #region Private Properties

        /// <summary>
        /// Gets the arranger chain.
        /// </summary>
        private ChainElementArranger ArrangerChain
        {
            get
            {
                if (_elementArrangerChain == null)
                {
                    lock (_codeArrangeChainLock)
                    {
                        if (_elementArrangerChain == null)
                        {
                            _elementArrangerChain = new ChainElementArranger();
                            foreach (ConfigurationElement configuration in _configuration.Elements)
                            {
                                IElementArranger elementArranger = ElementArrangerFactory.CreateElementArranger(
                                    configuration, _configuration);
                                if (elementArranger != null)
                                {
                                    _elementArrangerChain.AddArranger(elementArranger);
                                }
                            }
                        }
                    }
                }

                return _elementArrangerChain;
            }
        }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Arranges the code elements according to the configuration supplied 
        /// in the constructor.
        /// </summary>
        /// <param name="originalElements">Original elements</param>
        /// <returns>An arranged collection of code elements.</returns>
        public ReadOnlyCollection<ICodeElement> Arrange(ReadOnlyCollection<ICodeElement> originalElements)
        {
            GroupElement rootElement = new GroupElement();

            if (originalElements != null)
            {
                List<ICodeElement> elements = new List<ICodeElement>();
                NamespaceElement firstNamespace = null;
                for (int elementIndex = 0; elementIndex < originalElements.Count; elementIndex++)
                {
                    ICodeElement element = originalElements[elementIndex];
                    ICodeElement elementClone = element.Clone() as ICodeElement;
                    elements.Add(elementClone);

                    if (firstNamespace == null)
                    {
                        NamespaceElement namespaceElement = elementClone as NamespaceElement;
                        if (namespaceElement != null)
                        {
                            firstNamespace = namespaceElement;
                        }
                    }
                }

                MoveUsings(elements, firstNamespace);

                foreach (ICodeElement element in elements)
                {
                    ArrangerChain.ArrangeElement(rootElement, element);
                }
            }

            List<ICodeElement> arranged = new List<ICodeElement>(rootElement.Children);
            foreach (ICodeElement arrangedElement in arranged)
            {
                // Remove the root element as the parent.
                arrangedElement.Parent = null;
            }

            return arranged.AsReadOnly();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Moves using directives if configured to do so.
        /// </summary>
        /// <param name="elements">List of code elements.</param>
        /// <param name="namespaceElement">Namespace namespace to use when moving usings.</param>
        private void MoveUsings(List<ICodeElement> elements, NamespaceElement namespaceElement)
        {
            CodeLevel moveUsingsTo = _configuration.Formatting.Usings.MoveTo;
            if (moveUsingsTo != CodeLevel.None && namespaceElement != null)
            {
                if (moveUsingsTo == CodeLevel.Namespace)
                {
                    for (int elementIndex = 0; elementIndex < elements.Count; elementIndex++)
                    {
                        UsingElement usingElement = elements[elementIndex] as UsingElement;
                        if (usingElement != null && usingElement.IsMovable)
                        {
                            elements.Remove(usingElement);
                            namespaceElement.InsertChild(0, usingElement);
                            elementIndex--;
                        }
                    }
                }
                else if (moveUsingsTo == CodeLevel.File)
                {
                    for (int elementIndex = 0; elementIndex < namespaceElement.Children.Count; elementIndex++)
                    {
                        UsingElement usingElement = namespaceElement.Children[elementIndex] as UsingElement;
                        if (usingElement != null && usingElement.IsMovable)
                        {
                            namespaceElement.RemoveChild(usingElement);
                            elements.Insert(0, usingElement);
                            elementIndex--;
                        }
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException(
                        string.Format(
                        "Unknown code level '{0}'.",
                        moveUsingsTo));
                }
            }
        }

        #endregion Private Methods
    }
}