using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Services.Users
{
    /// <summary>
    /// ����������
    /// </summary>
    public static class UserAttributeExtensions
    {
        /// <summary>
        /// �������� ����������, ����� ���������������� ������� ������ ����� ��������
        /// </summary>
        /// <param name="UserAttribute">������� ������������</param>
        /// <returns>���������</returns>
        public static bool ShouldHaveValues(this UserAttribute UserAttribute)
        {
            if (UserAttribute == null)
                return false;

            if (UserAttribute.AttributeControlType == AttributeControlType.TextBox ||
                UserAttribute.AttributeControlType == AttributeControlType.MultilineTextbox ||
                UserAttribute.AttributeControlType == AttributeControlType.Datepicker ||
                UserAttribute.AttributeControlType == AttributeControlType.FileUpload)
                return false;

            //������ ������� ��� �������� ������������ ��������
            return true;
        }
    }
}
