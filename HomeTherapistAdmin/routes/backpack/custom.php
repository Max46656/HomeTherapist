<?php

use Illuminate\Support\Facades\Route;

// --------------------------
// Custom Backpack Routes
// --------------------------
// This route file is loaded automatically by Backpack\Base.
// Routes you generate using Backpack\Generators will be placed here.

Route::group([
    'prefix' => config('backpack.base.route_prefix', 'admin'),
    'middleware' => array_merge(
        (array) config('backpack.base.web_middleware', 'web'),
        (array) config('backpack.base.middleware_key', 'admin')
    ),
    'namespace' => 'App\Http\Controllers\Admin',
], function () { // custom admin routes

    Route::crud('appointment', 'AppointmentCrudController');
    Route::crud('appointment-detail', 'AppointmentDetailCrudController');
    Route::crud('article', 'ArticleCrudController');
    Route::crud('calendar', 'CalendarCrudController');
    Route::crud('feedback', 'FeedbackCrudController');
    Route::crud('order', 'OrderCrudController');
    Route::crud('order-detail', 'OrderDetailCrudController');
    Route::crud('service', 'ServiceCrudController');
    Route::crud('therapist-open-services', 'TherapistOpenServicesCrudController');
    Route::crud('therapist-open-time', 'TherapistOpenTimeCrudController');
    Route::crud('user', 'UserCrudController');
}); // this should be the absolute last line of this file
