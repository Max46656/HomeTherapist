<?php

namespace App\Http\Middleware;

use Closure;
use Illuminate\Support\Facades\Redirect;

class CheckIfUser
{

    public function handle($request, Closure $next)
    {
        if (backpack_user() && !backpack_user()->hasRole('admin')) {
            return Redirect::route('backpack.auth.logout');
        }

        return $next($request);
    }
}
