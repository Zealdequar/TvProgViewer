namespace TVProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// ������������ ��� �������� ��������
    /// </summary>
    public enum AttributeControlType
    {
        /// <summary>
        /// �������������� ������
        /// </summary>
        DropdownList = 1,

        /// <summary>
        /// ������ �����������
        /// </summary>
        RadioList = 2,

        /// <summary>
        /// ������
        /// </summary>
        Checkboxes = 3,

        /// <summary>
        /// ��������� ����
        /// </summary>
        TextBox = 4,

        /// <summary>
        /// ������������� ��������� ����
        /// </summary>
        MultilineTextbox = 10,

        /// <summary>
        /// ������� ���������� Datepicker
        /// </summary>
        Datepicker = 20,

        /// <summary>
        /// ������� ���������� ��������� ������
        /// </summary>
        FileUpload = 30,

        /// <summary>
        /// ������� ��������
        /// </summary>
        ColorSquares = 40,

        /// <summary>
        /// �����������
        /// </summary>
        ImageSquares = 45,

        /// <summary>
        /// ����� ������ ��� ������
        /// </summary>
        ReadonlyCheckboxes = 50
    }
}