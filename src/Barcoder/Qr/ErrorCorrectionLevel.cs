namespace Barcoder.Qr
{
    public enum ErrorCorrectionLevel
    {
        /// <summary>
        /// Recovers 7% of data.
        /// </summary>
        L,
        /// <summary>
        /// Recovers 15% of data.
        /// </summary>
        M,
        /// <summary>
        /// Recovers 25% of data.
        /// </summary>
        Q,
        /// <summary>
        /// Recovers 30% of data.
        /// </summary>
        H
    }
}
