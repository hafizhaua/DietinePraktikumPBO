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
    public class BreakfastFoodsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BreakfastFoodsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BreakfastFoods/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ClaimsPrincipal currentUser = this.User;
            var currentUserID = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            var breakfastFood = await _context.BreakfastFood
                .FirstOrDefaultAsync(m => m.BreakfastFoodID == id && m.BFUserID == currentUserID);

            if (breakfastFood == null)
            {
                return NotFound();
            }
            return View(breakfastFood);
        }

        // POST: BreakfastFoods/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BreakfastFoodID,BFName,BFCaloriePerOunce,BFGram,BFTotalCalorie,BFDbFoodID,BFUserID,BFDate")] BreakfastFood breakfastFood)
        {
            if (id != breakfastFood.BreakfastFoodID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string date = breakfastFood.BFDate;
                try
                {
                    _context.Update(breakfastFood);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BreakfastFoodExists(breakfastFood.BreakfastFoodID))
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
            return View(breakfastFood);
        }

        // GET: BreakfastFoods/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ClaimsPrincipal currentUser = this.User;
            var currentUserID = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            var breakfastFood = await _context.BreakfastFood
                .FirstOrDefaultAsync(m => m.BreakfastFoodID == id && m.BFUserID == currentUserID);
            if (breakfastFood == null)
            {
                return NotFound();
            }

            return View(breakfastFood);
        }

        // POST: BreakfastFoods/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var breakfastFood = await _context.BreakfastFood.FindAsync(id);

            string date = breakfastFood.BFDate;

            _context.BreakfastFood.Remove(breakfastFood);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), "MealPlans", new { date = date });

        }

        [Authorize]
        private bool BreakfastFoodExists(int id)
        {
            return _context.BreakfastFood.Any(e => e.BreakfastFoodID == id);
        }

        [Authorize]
        public async Task<IActionResult> Add(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ChosenFood = await _context.Food.FirstOrDefaultAsync(m => m.FoodID == id);

            var PlannedFood = new BreakfastFood
            {
                BFName = ChosenFood.Name,
                BFCaloriePerOunce = ChosenFood.CaloriePerOunce,
                BFDbFoodID = ChosenFood.FoodID,
            };

            if (TempData.ContainsKey("date"))
            {
                DateTime tanggalDT = DateTime.Parse(TempData["date"].ToString());
                PlannedFood.BFDate = tanggalDT.ToString("yyyy-MM-dd");
            }
            else PlannedFood.BFDate = DateTime.Now.ToString("yyyy-MM-dd");

            if (PlannedFood == null)
            {
                return NotFound();
            }

            return View(PlannedFood);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("BreakfastFoodID,BFName,BFCaloriePerOunce,BFGram,BFTotalCalorie,BFDbFoodID,BFUserID,BFDate")] BreakfastFood PlannedFood)
        {
            if (ModelState.IsValid)
            {
                ClaimsPrincipal currentUser = this.User;
                var currentUserID = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                PlannedFood.BFUserID = currentUserID;

                string date = PlannedFood.BFDate;

                _context.Add(PlannedFood);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "MealPlans", new { date = date });
            }
            return View(PlannedFood);
        }

        [Authorize]
        public IActionResult AddNewFood()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
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
