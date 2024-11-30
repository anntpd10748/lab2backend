using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using ServerGame106.Data;
using ServerGame106.Migrations;
using ServerGame106.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ServerGame106.DTO;
using Microsoft.AspNetCore.Identity.Data;
using ServerGame106.ViewModel;
using ServerGame106.Service;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;

namespace ServerGame106.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIGameController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        protected ResponseApi _response;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        public APIGameController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IEmailService emailService, IConfiguration configuration)
        {
            _db = db;
            _response = new();
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
        }
        [HttpGet("GetAllGameLevel")]
        public async Task<IActionResult> GetAllGameLevel()
        {
            try
            {
                var gamelevel = await _db.GameLevels.ToListAsync();
                _response.IsSuccess = true;
                _response.Notification = "lay du lieu thanh cong";
                _response.Data = gamelevel;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "loi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpGet("GetAllQuestionGame")]
        public async Task<IActionResult> GetAllQuestionGame()
        {
            try
            {
                var questionGame = await _db.Questions.ToListAsync();
                _response.IsSuccess = true;
                _response.Notification = "lay du lieu thanh cong";
                _response.Data = questionGame;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "loi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpGet("GetAllRegion")]
        public async Task<IActionResult> GetAllRegion()
        {
            try
            {
                var region = await _db.Regions.ToListAsync();
                _response.IsSuccess = true;
                _response.Notification = "lay du lieu thanh cong";
                _response.Data = region;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "loi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            try
            {
                var user = new ApplicationUser
                {
                    Email = registerDTO.Email,
                    UserName = registerDTO.Email,
                    Name = registerDTO.Name,
                    Avatar = registerDTO.LinkAvatar,
                    RegionId = registerDTO.RegionId
                };
                var result = await _userManager.CreateAsync(user, registerDTO.Password);
                if (result.Succeeded)
                {
                    _response.IsSuccess = true;
                    _response.Notification = "dang ky thanh cong";
                    _response.Data = user;
                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Notification = "dang ky that bai";
                    _response.Data = result.Errors;
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "loi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var email = loginRequest.Email;
                var password = loginRequest.Password;

                var user = await _userManager.FindByEmailAsync(email);
                if (user != null && await _userManager.CheckPasswordAsync(user, password))
                {
                    var token = GenerateJwtToken(user);
                    var data = new
                    {
                        token = token,
                        user = user
                    };
                    _response.IsSuccess = true;
                    _response.Notification = "dang nhap thanh cong";
                    _response.Data = data;
                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Notification = "dang nhap that bai";
                    _response.Data = null;
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "loi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpGet("GetAllQuestionGameByLevel/{levelId}")]
        public async Task<IActionResult> GetAllQuestionGameByLevel(int levelId)
        {
            try
            {
                var questionGame = await _db.Questions.Where(x => x.LevelId == levelId).ToListAsync();
                _response.IsSuccess = true;
                _response.Notification = "lay du lieu thanh cong";
                _response.Data = questionGame;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "loi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpPost("SaveResult")]
        public async Task<IActionResult> SaveResult(LevelResultDTO levelResult)
        {
            try
            {
                var levelResultSave = new LevelResult
                {
                    LevelId = levelResult.LevelId,
                    UserId = levelResult.UserId,
                    Score = levelResult.Score,
                    CompletionDate = DateOnly.FromDateTime(DateTime.Now)
                };
                await _db.LevelResults.AddAsync(levelResultSave);
                await _db.SaveChangesAsync();
                _response.IsSuccess = true;
                _response.Notification = "luu ket qua thanh cong";
                _response.Data = levelResult;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "loi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpGet("Rating/{idRegion}")]
        public async Task<IActionResult> Rating(int idRegion)
        {
            try
            {
                if(idRegion > 0)
                {
                    var nameRegion = await _db.Regions.Where(x => x.RegionId == idRegion).Select(x => x.Name).FirstOrDefaultAsync();
                    if(nameRegion == null)
                    {
                        _response.IsSuccess = false;
                        _response.Notification = "khong tim thay khu vuc";
                        _response.Data = null;
                        return BadRequest(_response);
                    }
                    var userByRegion = await _db.Users.Where(x => x.RegionId == idRegion).ToListAsync();
                    var resultLevelByRegion = await _db.LevelResults.Where(x => userByRegion.Select(x => x.Id).Contains(x.UserId)).ToListAsync();
                    RatingVM ratingVM = new();
                    ratingVM.NameRegion = nameRegion;
                    ratingVM.userResultSums = new();
                    foreach(var item in userByRegion)
                    {
                        var sumScore = resultLevelByRegion.Where(x => x.UserId == item.Id).Sum(x => x.Score);
                        var sumLevel = resultLevelByRegion.Where(x => x.UserId == item.Id).Count();
                        UserResultSum userResultSum = new();
                        userResultSum.NameUser = item.Name;
                        userResultSum.SumScore = sumScore;
                        userResultSum.SumLevel = sumLevel;
                        ratingVM.userResultSums.Add(userResultSum);
                    }
                    _response.IsSuccess = true;
                    _response.Notification = "lay du lieu thanh cong";
                    _response.Data = ratingVM;
                    return Ok(_response);
                }
                else
                {
                    var user = await _db.Users.ToListAsync();
                    var resultLevel = await _db.LevelResults.ToListAsync();
                    string nameRegion = "tat ca";
                    RatingVM ratingVM = new();
                    ratingVM.NameRegion = nameRegion;
                    ratingVM.userResultSums = new();
                    foreach(var item in user)
                    {
                        var sumScore = resultLevel.Where(x => x.UserId == item.Id).Sum(x => x.Score);
                        var sumLevel = resultLevel.Where(x => x.UserId == item.Id).Count();
                        UserResultSum userResultSum = new();
                        userResultSum.NameUser = item.Name;
                        userResultSum.SumScore = sumScore;
                        userResultSum.SumLevel = sumLevel;
                        ratingVM.userResultSums.Add(userResultSum);
                    }
                    _response.IsSuccess = true;
                    _response.Notification = "lay du lieu thanh cong";
                    _response.Data = ratingVM;
                    return Ok(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "loi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpGet("GetUserInformation/{userId}")]
        public async Task<IActionResult> GetUserInformation(string userId)
        {
            try
            {
                var user = await _db.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "khong tim thay nguoi dung";
                    _response.Data = null;
                    return BadRequest(_response);
                }
                UserInformationVM userInformationVM = new();
                userInformationVM.Name = user.Name;
                userInformationVM.Email = user.Email;
                userInformationVM.avatar = user.Avatar;
                userInformationVM.Region = await _db.Regions.Where(x => x.RegionId == user.RegionId).Select(x => x.Name).FirstOrDefaultAsync();
                _response.IsSuccess = true;
                _response.Notification = "lay du lieu thanh cong";
                _response.Data = userInformationVM;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "loi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpPut("ChangeUserPassword")]
        public async Task<IActionResult> ChangeUserPassword(ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                var user =await _db.Users.Where(x => x.Id == changePasswordDTO.UserId).FirstOrDefaultAsync();
                if(user == null)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "khong tim thay nguoi dung";
                    _response.Data = null;
                    return BadRequest(_response);
                }
                var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);
                if(result.Succeeded)
                {
                    _response.IsSuccess = true;
                    _response.Notification = "doi mat khau thanh cong";
                    _response.Data = "";
                    return Ok (_response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Notification = "doi mat khau that bai";
                    _response.Data = result.Errors;
                    return BadRequest(_response);
                }
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "loi";
                _response.Data=ex.Message;
                return BadRequest(_response);   
            }
        }
        [HttpPut("UpdateUserInformation")]
        public async Task<IActionResult> UpdateUserInformation([FromForm] UserInformationDTO userInformationDTO)
        {
            try
            {
                var user = await _db.Users.Where(x => x.Id == userInformationDTO.UserId).FirstOrDefaultAsync();
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "khong tim thay nguoi dung";
                    _response.Data = null;
                    return BadRequest(_response);
                }

                user.Name = userInformationDTO.Name;
                user.RegionId = userInformationDTO.RegionId;

                if(userInformationDTO.Avatar != null)
                {
                    var fileExtension = Path.GetExtension(userInformationDTO.Avatar.FileName);
                    var fileName = $"{userInformationDTO.UserId}{fileExtension}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/avatars", fileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    using(var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await userInformationDTO.Avatar.CopyToAsync(stream);
                    }

                    user.Avatar = fileName;
                }
                await _db.SaveChangesAsync();
                _response.IsSuccess = true;
                _response.Notification = "cap nhat thong tin thanh cong";
                _response.Data = user;
                return Ok (_response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "loi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpDelete("DeleteAccount/{userId}")]
        public async Task<IActionResult> DeleteAccount(string userId)
        {
            try
            {
                var user = await _db.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
                if(user == null)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "khong tim that nguoi dung";
                    _response.Data = null;
                    return BadRequest(_response);
                }
                user.IsDeleted = true;
                await _db.SaveChangesAsync();
                _response.IsSuccess = true;
                _response.Notification = "xoa nguoi dung thanh cong";
                _response.Data = user;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "loi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(Email);
                if(user == null)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "khong tim that nguoi dung";
                    _response.Data = null;
                    return BadRequest(_response);
                }
                Random random = new();
                string OTP = random.Next(100000, 999999).ToString();
                user.OTP = OTP;
                await _userManager.UpdateAsync(user);
                await _db.SaveChangesAsync();
                string subject = "reset password GA105 - " + Email;
                string message = "ma OTP cua ban la; " + OTP;
                await _emailService.SendEmailAsync(Email, subject, message);
                _response.IsSuccess = true;
                _response.Notification = "gui ma OTP thanh cong";
                _response.Data = "email send to " + Email;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "loi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpPost("CheckOTP")]
        public async Task<IActionResult> CheckOTP(CheckOTPDTO checkOTPDTO)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(checkOTPDTO.Email);
                if(user == null)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "khong tim that nguoi dung";
                    _response.Data = null;
                    return BadRequest(_response);
                }
                var stringOTP = Convert.ToInt32(checkOTPDTO.OTP).ToString();
                if(stringOTP == user.OTP)
                {
                    _response.IsSuccess = true;
                    _response.Notification = "ma OTP chinh xac";
                    _response.Data = user.Email;
                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Notification = "ma OTP khong chinh xac";
                    _response.Data = null;
                    return BadRequest(_response);
                }
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "loi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);
                if(user == null)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "khong tim that nguoi dung";
                    _response.Data = null;
                    return BadRequest(_response);
                }
                var stringOTP = Convert.ToInt32(resetPasswordDTO.OTP).ToString();
                if(stringOTP == user.OTP)
                {
                    DateTime now = DateTime.Now;
                    user.OTP = $"{stringOTP}_used_" + now.ToString("yyyy_MM_dd_HH_mm_ss");

                    var passwordHasher = new PasswordHasher<IdentityUser>();
                    user.PasswordHash = passwordHasher.HashPassword(user, resetPasswordDTO.NewPassword);

                    var result = await _userManager.UpdateAsync(user);
                    if(result.Succeeded)
                    {
                        _response.IsSuccess = true;
                        _response.Notification = "doi mat khau thanh cong";
                        _response.Data = resetPasswordDTO.Email;
                        return Ok(_response);
                    }
                    else
                    {
                        _response.IsSuccess = false;
                        _response.Notification = "doi mat khau that bai";
                        _response.Data = result.Errors;
                        return BadRequest(_response);
                    }
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Notification = "ma OTP khong chinh xac ";
                    _response.Data = null;
                    return BadRequest(_response);
                }
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "loi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        private string GenerateJwtToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires:DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [HttpGet("GetAllResultByUser/{userId}")]
        //[Authorize]
        public async Task<IActionResult> GetAllResultByUser(string userId)
        {
            try
            {
                var result = await _db.LevelResults.Where(x =>  x.UserId == userId).ToListAsync();

                _response.IsSuccess = true;
                _response.Notification = "lay du lieu thanh cong";
                _response.Data = result;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "loi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
    }
}
