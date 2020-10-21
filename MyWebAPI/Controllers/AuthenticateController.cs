using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using MyWebAPI.Model;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyWebAPI.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticateController : Controller
    {
        private readonly TokenManagement _token;
        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="token"></param>
        public AuthenticateController(IOptions<TokenManagement> token)
        {
            _token = token.Value;
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginInput input)
        {
            //从数据库验证用户名，密码 
            //验证通过 否则 返回Unauthorized
            if (input.Username != "admin")
                return BadRequest("用户名或密码错误");

            //创建claim
            var authClaims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub,input.Username),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            IdentityModelEventSource.ShowPII = true;
            //签名秘钥 可以放到json文件中
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_token.Secret));

            var token = new JwtSecurityToken(
                   issuer: _token.Issuer,
                   audience: _token.Audience,
                   expires: DateTime.Now.AddDays(1),
                   claims: authClaims,
                   signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                   );

            //返回token和过期时间
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
    }
}
