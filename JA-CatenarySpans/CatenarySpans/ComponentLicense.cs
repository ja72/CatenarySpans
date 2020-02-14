using System;
using System.Linq;
using System.ComponentModel;

namespace JA.CatenarySpans
{
    /// <summary>
    /// License granted to components.
    /// </summary>
    public class ComponentLicense : License
    {

        #region Documentation

        /*
         * This is a very simple example. You could expand this example by storing more information
         * in the license key. For example, you could store a byte array of license information such
         * as instances, features, etc., then change that to a base-64 number that can be saved and
         * retrieved. Using a CRC or some other embedded check helps to keep fakes at bay. You can
         * get as creative as you want in the VerifyKey function, such as only allow the user to use
         * the control on certain dates, or it could even check a web service for subscription based
         * licenses. 
         * 
         * It is important to note though to keep as much of your code as possible private or internal
         * so that your obfuscation programs can really mangle the names. This will keep hackers from
         * easily reverse-engineering or disabling your license system.
        */

        #endregion

        #region Fields

        private string _licKey = string.Empty;
        private bool _isDemo = false;
        private bool _disposed = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the license key granted to this component.
        /// </summary>
        public override string LicenseKey
        {
            get { return _licKey; }
        }

        /// <summary>
        /// Gets if this component is running in demo mode.
        /// </summary>
        public bool IsDemo
        {
            get { return _isDemo; }
        }

        #endregion

        #region Construction / Deconstruction

        /// <summary>
        /// Creates a new <see cref="ComponentLicense"/> object.
        /// </summary>
        /// <param name="licKey">License key to use.</param>
        private ComponentLicense(string licKey)
        {
            _licKey = licKey;

            if (!VerifyKey())
                _isDemo = true;
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        public sealed override void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        /// <param name="disposing">true if the object is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_disposed)
                {
                    //Custom disposing here.
                }
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a demo <see cref="ComponentLicense"/>.
        /// </summary>
        /// <returns>A demo <see cref="ComponentLicense"/>.</returns>
        internal static ComponentLicense CreateDemoLicense()
        {
            return new ComponentLicense(string.Empty);
        }

        /// <summary>
        /// Attempts to create a new <see cref="ComponentLicense"/> with the specified key.
        /// </summary>
        /// <param name="developerKey">Developer Key</param>
        /// <returns><see cref="ComponentLicense"/> with the specified fields set.</returns>
        internal static ComponentLicense CreateLicense(string developerKey)
        {
            return new ComponentLicense(developerKey);
        }

        #endregion

        #region Private Methods

        private bool VerifyKey()
        {
            //Implement your own key verification here. For now we will
            //just verify that the last 8 characters are a valid CRC.
            //The key is a simple Guid with an extended 8 characters for
            //the CRC.

            if (string.IsNullOrEmpty(_licKey))
                return false;

            //Guid's are in the following format:
            //F2A7629C-5AAF-4E86-8EC2-64F73B6A4FE3
            //Developer keys are an extension of that, like:
            //F2A7629C-5AAF-4E86-8EC2-64F73B6A4FE3-XXXXXXXX

            //So a developer key MUST be 45 characters long

            if (_licKey.Length != 45)
                return false;

            //It must also contain -'s
            if (!_licKey.Contains('-'))
                return false;

            //Now split it
            string[] splitKey = _licKey.Split('-');

            //It has to have 6 parts or its invalid
            if (splitKey.Length != 6)
                return false;

            //Join elements 1 through 5, then convert to a byte array
            string baseKey = string.Join("-", splitKey, 0, 5);
            byte[] asciiBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(baseKey);

            //Get the CRC
            uint baseCRC = Crc32.Compute(asciiBytes);

            //Now let's compare the CRC to make sure its a valid key
            if (string.Equals(splitKey[5], baseCRC.ToString("X8"), StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        #endregion

    }
}
