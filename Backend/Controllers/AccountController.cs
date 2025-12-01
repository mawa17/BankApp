using Backend.Data;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;


[ApiController]
[Route("api/[controller]/[action]")]
public class AccountController(IAccountService service) : ControllerBase
{

}
