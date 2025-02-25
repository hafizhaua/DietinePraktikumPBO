﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DietineWebApp.Data;
using DietineWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DietineWebApp.Controllers
{
    public class DinnerFoodsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DinnerFoodsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DinnerFoods/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ClaimsPrincipal currentUser = this.User;
            var currentUserID = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            var dinnerFood = await _context.DinnerFood
                .FirstOrDefaultAsync(m => m.DinnerFoodID == id && m.DFUserID == currentUserID);

            if (dinnerFood == null)
            {
                return NotFound();
            }
            return View(dinnerFood);
        }

        // POST: DinnerFoods/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DinnerFoodID,DFName,DFCaloriePerOunce,DFGram,DFTotalCalorie,DFDbFoodID,DFUserID,DFDate")] DinnerFood dinnerFood)
        {
            if (id != dinnerFood.DinnerFoodID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string date = dinnerFood.DFDate;
                try
                {
                    _context.Update(dinnerFood);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DinnerFoodExists(dinnerFood.DinnerFoodID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), "MealPlans", new { date = date });
            }
            return View(dinnerFood);
        }

        // GET: DinnerFoods/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ClaimsPrincipal currentUser = this.User;
            var currentUserID = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            var dinnerFood = await _context.DinnerFood
                .FirstOrDefaultAsync(m => m.DinnerFoodID == id && m.DFUserID == currentUserID);
            if (dinnerFood == null)
            {
                return NotFound();
            }

            return View(dinnerFood);
        }

        // POST: DinnerFoods/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dinnerFood = await _context.DinnerFood.FindAsync(id);
            string date = dinnerFood.DFDate;
            _context.DinnerFood.Remove(dinnerFood);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), "MealPlans", new { date = date });
        }

        [Authorize]
        private bool DinnerFoodExists(int id)
        {
            return _context.DinnerFood.Any(e => e.DinnerFoodID == id);
        }

        [Authorize]
        public async Task<IActionResult> Add(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ChosenFood = await _context.Food.FirstOrDefaultAsync(m => m.FoodID == id);

            var PlannedFood = new DinnerFood
            {
                DFName = ChosenFood.Name,
                DFCaloriePerOunce = ChosenFood.CaloriePerOunce,
                DFDbFoodID = ChosenFood.FoodID
            };

            if (TempData.ContainsKey("date"))
            {
                DateTime tanggalDT = DateTime.Parse(TempData["date"].ToString());
                PlannedFood.DFDate = tanggalDT.ToString("yyyy-MM-dd");
            }
            else PlannedFood.DFDate = DateTime.Now.ToString("yyyy-MM-dd");

            if (PlannedFood == null)
            {
                return NotFound();
            }

            return View(PlannedFood);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Add([Bind("DinnerFoodID,DFName,DFCaloriePerOunce,DFGram,DFTotalCalorie,DFDbFoodID,DFUserID,DFDate")] DinnerFood PlannedFood)
        {
            if (ModelState.IsValid)
            {
                ClaimsPrincipal currentUser = this.User;
                var currentUserID = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                PlannedFood.DFUserID = currentUserID;

                string date = PlannedFood.DFDate;
                _context.Add(PlannedFood);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "MealPlans", new { date = date });
            }
            return View(PlannedFood);
        }

        public IActionResult AddNewFood()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddNewFood([Bind("FoodID,Name,CaloriePerOunce")] Food food)
        {
            if (ModelState.IsValid)
            {
                _context.Food.Add(food);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(SeeList));
            }
            return View(food);
        }

        [Authorize]
        public async Task<IActionResult> SeeList()
        {
            return View(await _context.Food.ToListAsync());
        }
    }
}
