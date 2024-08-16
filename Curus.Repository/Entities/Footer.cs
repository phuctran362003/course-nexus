using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Entities
{
    public class Footer : BaseEntity<int>
    {
        public string PhoneNumber { get; set; } = "(+84) 28 7108 3338";
        public string Address { get; set; } = "Lô C1-2, đường D1, Khu Công nghệ cao Tp. HCM Phường Tân Phú, Tp. Thủ Đức, Tp. Hồ Chí Minh";
        public string WorkingTime { get; set; } = "6:00:00 - 19:00:00";

        public string Privacy { get; set; } = "Chính sách bảo mật: Chúng tôi cam kết bảo vệ quyền riêng tư của bạn. Mọi thông tin cá nhân thu thập sẽ được sử dụng đúng mục đích và bảo mật tuyệt đối.";
        public string Term_of_use { get; set; } = "Điều khoản sử dụng: Vui lòng đọc kĩ các điều khoản trước khi sử dụng dịch vụ của chúng tôi. Bằng việc sử dụng dịch vụ, bạn đồng ý tuân thủ các quy định và điều kiện được đề cập.";
    }

}
