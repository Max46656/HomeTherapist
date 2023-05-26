<?php

namespace App\Providers;

use Illuminate\Support\ServiceProvider;

class AppServiceProvider extends ServiceProvider
{
    /**
     * Register any application services.
     */
    public function register(): void
    {
        //
    }

    /**
     * Bootstrap any application services.
     */
    public function boot(): void
    {
        // $faker = FakerFactory::create('zh_TW');
        // $this->app->singleton(\Faker\Generator::class, function () use ($faker) {
        //     return $faker;
        // });
    }
}
