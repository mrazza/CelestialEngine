// -----------------------------------------------------------------------
// <copyright file="EngineContent.cs" company="">
// Copyright (C) 2013 Matthew Razza & Will Graham
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Content
{
    namespace Shaders
    {
        /// <summary>
        /// Contains Core shader content paths.
        /// </summary>
        public static class Core
        {
            /// <summary>
            /// The content path for the merge targets shader.
            /// </summary>
            public const string MergeTargets = @"Content\Shaders\Core\MergeTargets";

            /// <summary>
            /// The content path for the debug targets shader.
            /// </summary>
            public const string DebugTargets = @"Content\Shaders\Core\DebugTargets";

            /// <summary>
            /// The content path for the normal map shader.
            /// </summary>
            public const string NormalMapShader = @"Content\Shaders\Core\NormalMap";

            /// <summary>
            /// The content path for the options map flags shader.
            /// </summary>
            public const string OptionsMapFlags = @"Content\Shaders\Core\OptionsMapFlags";

            /// <summary>
            /// The content path for the shadow map shader.
            /// </summary>
            public const string ShadowMap = @"Content\Shaders\Core\ShadowMap";
        }

        /// <summary>
        /// Contains Lights shader content paths.
        /// </summary>
        public static class Lights
        {
            /// <summary>
            /// The content path for the ambient light shader.
            /// </summary>
            public const string AmbientLight = @"Content\Shaders\Lights\AmbientLight";

            /// <summary>
            /// The content path for the cone light shader.
            /// </summary>
            public const string ConeLight = @"Content\Shaders\Lights\ConeLight";

            /// <summary>
            /// The content path for the point light shader.
            /// </summary>
            public const string PointLight = @"Content\Shaders\Lights\PointLight";

            /// <summary>
            /// The content path for the rectangular light shader.
            /// </summary>
            public const string RectangularLight = @"Content\Shaders\Lights\RectangularLight";
        }
        
        /// <summary>
        /// Contains Simple shader content paths.
        /// </summary>
        public static class Simple
        {
            /// <summary>
            /// The content path for the ambient light shader.
            /// </summary>
            public const string InvertColors = @"Content\Shaders\Simple\InvertColors";
        }
    }
}
